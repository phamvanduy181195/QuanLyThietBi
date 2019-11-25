using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewCM.Authorization;
using NewCM.Authorization.Users;
using NewCM.DbEntities;
using NewCM.Global;
using NewCM.Global.Dto;
using NewCM.KhachHangs.Dto;
using NewCM.KhachHangs.Importing;
using NewCM.Net.MimeTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NewCM.KhachHangs
{
    [AbpAuthorize(PermissionNames.Pages_KhachHang)]
    public class KhachHangAppService : NewCMAppServiceBase, IKhachHangAppService
    {
        private readonly IRepository<KhachHang, long> _khachHangRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly LogInManager _logInManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<DanhMucTinhThanh> _tinhThanhRepository;
        private readonly IRepository<DanhMucQuanHuyen> _quanHuyenRepository;
        private readonly IGlobalCache _globalCache;

        private readonly IAppFolders _appFolders;
        private readonly IKhachHangExcelImporter _khachHangExcelImporter;

        public KhachHangAppService(IRepository<KhachHang, long> khachHangRepository,
            IRepository<User, long> userRepository,
            UserRegistrationManager userRegistrationManager,
            LogInManager logInManager,
            IPasswordHasher<User> passwordHasher,
            IRepository<DanhMucTinhThanh> tinhThanhRepository,
            IRepository<DanhMucQuanHuyen> quanHuyenRepository,
            IGlobalCache globalCache,

            IAppFolders appFolders,
            IKhachHangExcelImporter khachHangExcelImporter
        )
        {
            _khachHangRepository = khachHangRepository;
            _userRepository = userRepository;
            _userRegistrationManager = userRegistrationManager;
            _logInManager = logInManager;
            _passwordHasher = passwordHasher;
            _tinhThanhRepository = tinhThanhRepository;
            _quanHuyenRepository = quanHuyenRepository;
            _globalCache = globalCache;

            _appFolders = appFolders;
            _khachHangExcelImporter = khachHangExcelImporter;
        }

        public async Task<PagedResultDto<KhachHangDto>> GetAll(GetAllKhachHangInput input)
        {
            var query = from KhachHang in _khachHangRepository.GetAll()
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), w => w.Name.Contains(input.Filter) || w.PhoneNumber.Contains(input.Filter))

                        join TaiKhoan in _userRepository.GetAll() on KhachHang.UserId equals TaiKhoan.Id into J1
                        from TaiKhoan in J1.DefaultIfEmpty()

                        join TinhThanh in _tinhThanhRepository.GetAll() on KhachHang.ProvinceId equals TinhThanh.Id into J3
                        from TinhThanh in J3.DefaultIfEmpty()

                        join QuanHuyen in _quanHuyenRepository.GetAll() on KhachHang.DistrictId equals QuanHuyen.Id into J4
                        from QuanHuyen in J4.DefaultIfEmpty()

                        select new
                        {
                            KhachHang,
                            TaiKhoan.UserName,
                            IsActive = TaiKhoan != null ? TaiKhoan.IsActive.ToString() : "",
                            TaiKhoan.EmailConfirmationCode,
                            TinhThanhName = TinhThanh.Name,
                            QuanHuyenName = QuanHuyen.Name
                        };

            var TotalCount = await query.CountAsync();
            var PagedKhachHang = await query.OrderBy(o => o.KhachHang.Name).PageBy(input).ToListAsync();

            var KhachHangs = new List<KhachHangDto>();
            foreach (var item in PagedKhachHang)
            {
                var KhachHang = ObjectMapper.Map<KhachHangDto>(item.KhachHang);
                KhachHang.UserName = item.UserName;
                KhachHang.AddressFull = KhachHang.Address + (string.IsNullOrWhiteSpace(item.QuanHuyenName) ? "" : ", " + item.QuanHuyenName);
                KhachHang.AddressFull += string.IsNullOrWhiteSpace(item.TinhThanhName) ? "" : ", " + item.TinhThanhName;

                if (!string.IsNullOrWhiteSpace(item.IsActive))
                {
                    // Không active và không có email confirm code
                    // => Đã active, đang bị lock
                    if (item.IsActive == "False" && string.IsNullOrWhiteSpace(item.EmailConfirmationCode))
                    {
                        KhachHang.IsActived = true;
                        KhachHang.IsLocked = true;

                        KhachHang.Status = "Đã khóa";
                    }
                    // Không active và có email confirm code
                    // => Đang chờ active, không bị lock
                    else if (item.IsActive == "False" && !string.IsNullOrWhiteSpace(item.EmailConfirmationCode))
                    {
                        KhachHang.IsActived = false;
                        KhachHang.IsLocked = false;

                        KhachHang.Status = "Chưa kích hoạt";
                    }
                    // Có active, không quan tâm email confirm code
                    // => Đang active, không bị lock
                    else
                    {
                        KhachHang.IsActived = true;
                        KhachHang.IsLocked = false;

                        KhachHang.Status = "Đang hoạt động";
                    }
                }
                else
                {
                    KhachHang.Status = "-";
                }

                KhachHangs.Add(KhachHang);
            }

            return new PagedResultDto<KhachHangDto>(
                TotalCount,
                KhachHangs
            );
        }

        public async Task<KhachHangDto> Get(EntityDto<long> input)
        {
            var result = await _khachHangRepository.FirstOrDefaultAsync(input.Id);

            if (result == null)
                throw new UserFriendlyException(L("KhachHangIsNotFound"));

            return ObjectMapper.Map<KhachHangDto>(result);
        }

        public async Task Create(CreateKhachHangDto input)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                input.PhoneNumber,
                input.EmailAddress,
                "",
                input.Password,
                true,          // Not active by default
                true
            );

            // Ghép chuỗi để có địa chỉ đầy đủ
            string Address = input.Address;
            string QuanHuyenName = await _globalCache.GetQuanHuyenName(input.ProvinceId, input.DistrictId);
            Address += string.IsNullOrWhiteSpace(QuanHuyenName) ? "" : ", " + QuanHuyenName;
            string TinhThanhName = await _globalCache.GetTinhThanhName(input.ProvinceId);
            Address += string.IsNullOrWhiteSpace(TinhThanhName) ? "" : ", " + TinhThanhName;
            Address += ", Việt Nam";

            // Kiểm tra bên bảng KhachHang, nếu có bản ghi trùng SĐT mà chưa kết nối vào tài khoản nào thì tiến hành kết nối
            KhachHang newKhachHang = await _khachHangRepository.FirstOrDefaultAsync(w => w.PhoneNumber == input.PhoneNumber && w.UserId == null);
            if (newKhachHang == null)
            {
                newKhachHang = new KhachHang
                {
                    Name = input.Name,
                    Address = input.Address,
                    ProvinceId = input.ProvinceId,
                    DistrictId = input.DistrictId,
                    Location = await GlobalFunction.GetLongLatFromAddress(_globalCache.GetGoogleApiKey(), Address),
                    PhoneNumber = input.PhoneNumber,
                    EmailAddress = input.EmailAddress,
                    UserId = user.Id
                };
                await _khachHangRepository.InsertAsync(newKhachHang);
            }
            else
            {
                newKhachHang.Name = input.Name;
                newKhachHang.Address = input.Address;
                newKhachHang.ProvinceId = input.ProvinceId;
                newKhachHang.DistrictId = input.DistrictId;
                newKhachHang.Location = await GlobalFunction.GetLongLatFromAddress(_globalCache.GetGoogleApiKey(), Address);
                newKhachHang.PhoneNumber = input.PhoneNumber;
                newKhachHang.EmailAddress = input.EmailAddress;
                newKhachHang.UserId = user.Id;
            }

            //return ObjectMapper.Map<KhachHangDto>(newKhachHang);
        }

        [HttpPost]
        public async Task Update(KhachHangDto input)
        {
            var updateKhachHang = await _khachHangRepository.FirstOrDefaultAsync(input.Id);

            if (updateKhachHang == null)
                throw new UserFriendlyException(L("KhachHangIsNotFound"));

            // Ghép chuỗi để có địa chỉ đầy đủ
            string Address = input.Address;
            string QuanHuyenName = await _globalCache.GetQuanHuyenName(input.ProvinceId, input.DistrictId);
            Address += string.IsNullOrWhiteSpace(QuanHuyenName) ? "" : ", " + QuanHuyenName;
            string TinhThanhName = await _globalCache.GetTinhThanhName(input.ProvinceId);
            Address += string.IsNullOrWhiteSpace(TinhThanhName) ? "" : ", " + TinhThanhName;
            Address += ", Việt Nam";

            // Update data
            updateKhachHang.Name = input.Name;
            updateKhachHang.EmailAddress = input.EmailAddress;
            updateKhachHang.Address = input.Address;
            updateKhachHang.DistrictId = input.DistrictId;
            updateKhachHang.ProvinceId = input.ProvinceId;
            updateKhachHang.Location = await GlobalFunction.GetLongLatFromAddress(_globalCache.GetGoogleApiKey(), Address);

            if (updateKhachHang.UserId > 0)
            {
                var updateUser = await _userRepository.FirstOrDefaultAsync((long)updateKhachHang.UserId);
                if (updateUser != null)
                {
                    updateUser.Name = updateKhachHang.Name;
                    updateUser.EmailAddress = updateKhachHang.EmailAddress;
                }
            }
        }

        public async Task Delete(EntityDto<long> input)
        {
            var deleteKhachHang = await _khachHangRepository.FirstOrDefaultAsync(input.Id);

            if (deleteKhachHang == null)
                throw new UserFriendlyException(L("KhachHangIsNotFound"));

            if (deleteKhachHang.UserId > 0)
                await _userRepository.DeleteAsync((long)deleteKhachHang.UserId);

            await _khachHangRepository.DeleteAsync(input.Id);
        }

        public async Task ActiveAccount(EntityDto<long> input)
        {
            var currentUser = await _userRepository.FirstOrDefaultAsync(input.Id);

            if (currentUser != null)
            {
                if (currentUser.IsActive)
                    throw new UserFriendlyException(L("UserIsActivated"));

                if (string.IsNullOrWhiteSpace(currentUser.EmailConfirmationCode))
                    throw new UserFriendlyException(L("UserIsLocked"));

                currentUser.IsActive = true;
                currentUser.EmailConfirmationCode = null;
            }
            else
            {
                throw new UserFriendlyException(L("AccountIsNotFound"));
            }
        }

        public async Task LockUnlockAccount(LockUnlockInput input)
        {
            var currentUser = await _userRepository.FirstOrDefaultAsync(input.UserId);

            if (currentUser != null)
            {
                // Kiểm tra nếu tài khoản chưa kích hoạt thì báo lỗi
                if (!currentUser.IsActive && !string.IsNullOrWhiteSpace(currentUser.EmailConfirmationCode))
                    throw new UserFriendlyException(L("UserIsNotActive"));

                // Yêu cầu khóa
                if (input.IsLocked)
                {
                    // Kiểm tra nếu chưa khóa thì khóa lại
                    if (currentUser.IsActive)
                    {
                        currentUser.IsActive = false;
                    }
                }
                // Yêu cầu mở khóa
                else
                {
                    // Kiểm tra nếu đang khóa thì mở khóa
                    if (!currentUser.IsActive)
                    {
                        currentUser.IsActive = true;
                    }
                }

                currentUser.EmailConfirmationCode = null;
            }
            else
            {
                throw new UserFriendlyException(L("AccountIsNotFound"));
            }
        }

        public async Task SetPassword(SetPasswordDto input)
        {
            // Kiểm tra password admin
            var adminUser = await _userRepository.FirstOrDefaultAsync((long)AbpSession.UserId);
            var loginAsync = await _logInManager.LoginAsync(adminUser.UserName, input.AdminPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException(L("WrongAdminPassword"));
            }

            var currentUser = await _userRepository.FirstOrDefaultAsync(input.UserId);

            if (currentUser != null)
            {
                currentUser.Password = _passwordHasher.HashPassword(currentUser, input.Password);
            }
            else
            {
                throw new UserFriendlyException(L("AccountIsNotFound"));
            }
        }

        public async Task CreateAccount(EntityDto<long> input)
        {
            var currentKhachHang = await _khachHangRepository.FirstOrDefaultAsync(input.Id);

            if (currentKhachHang != null)
            {
                var checkUser = await _userRepository.FirstOrDefaultAsync(w => w.UserName == currentKhachHang.PhoneNumber);

                // Nếu số điện thoại của KH đã có tài khoản
                //  => Kiểm tra xem tài khoản đã được sử dụng cho khách hàng khách chưa
                //      => Nếu chưa được sử dụng => Sử dụng luôn cho KH này
                //      => Nếu đã được sử dụng => Báo lỗi
                if (checkUser != null)
                {
                    var checkKhachHang = await _khachHangRepository.FirstOrDefaultAsync(w => w.UserId == checkUser.Id);
                    if (checkKhachHang != null)
                        throw new UserFriendlyException("");

                    checkUser.PhoneNumber = currentKhachHang.PhoneNumber;
                    checkUser.EmailAddress = currentKhachHang.EmailAddress;
                    checkUser.Password = _passwordHasher.HashPassword(checkUser, "123456");

                    currentKhachHang.UserId = checkUser.Id;
                }
                else
                {
                    var newUser = await _userRegistrationManager.RegisterAsync(
                                        currentKhachHang.PhoneNumber,
                                        currentKhachHang.EmailAddress,
                                        "",
                                        "123456",
                                        true,
                                        true
                                    );

                    currentKhachHang.UserId = newUser.Id;
                }
            }
            else
            {
                throw new UserFriendlyException(L("KhachHangIsNotFound"));
            }
        }

        public async Task<string> ImportFileExcel(string FileName)
        {
            string ReturnMessage = "Kết quả nhập file:";

            string FilePath = Path.Combine(_appFolders.KhachHangImportFolder, FileName);

            if (!File.Exists(FilePath))
            {
                return "Có lỗi: Không upload được file lên server!";
            }

            var ReadResult = _khachHangExcelImporter.ReadFromExcel(FilePath);

            if (ReadResult.ResultCode != 200)
            {
                return ReadResult.ErrorMessage;
            }
            else
            {
                ReturnMessage += string.Format("\r\n\u00A0- Tổng số bản ghi: {0}", ReadResult.ListResult.Count + ReadResult.ListErrorRow.Count);
                ReturnMessage += string.Format("\r\n\u00A0- Số bản ghi thành công: {0}", ReadResult.ListResult.Count);
                ReturnMessage += string.Format("\r\n\u00A0- Số bản ghi lỗi: {0}", ReadResult.ListErrorRow.Count);

                // Insert vào bảng KhachHang
                foreach (var khachhang in ReadResult.ListResult)
                {
                    await _khachHangRepository.InsertAsync(khachhang);
                }
            }

            return ReturnMessage;
        }

        public async Task<FileDto> DownloadFileMau()
        {
            string FileName = string.Format("ImportKhachHang.xlsx");

            var result = new FileDto(FileName, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);

            string SourceFile = Path.Combine(_appFolders.ImportSampleFolder, FileName);
            string DestinationFile = Path.Combine(_appFolders.TempFileDownloadFolder, result.FileToken);

            try
            {
                File.Copy(SourceFile, DestinationFile, true);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Có lỗi: " + ex.Message);
            }
        }
    }
}
