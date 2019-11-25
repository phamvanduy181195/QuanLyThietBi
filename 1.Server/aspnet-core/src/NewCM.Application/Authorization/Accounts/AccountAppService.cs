using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using NewCM.Authorization.Accounts.Dto;
using NewCM.Authorization.Users;
using NewCM.DbEntities;
using NewCM.Global;
using NewCM.Users.Dto;
using NewCM.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace NewCM.Authorization.Accounts
{
    public class AccountAppService : NewCMAppServiceBase, IAccountAppService
    {
        // from: http://regexlib.com/REDetails.aspx?regexp_id=1923
        public const string PasswordRegex = "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\\s)[0-9a-zA-Z!@#$%^&*()]*$";

        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly UserManager _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAbpSession _abpSession;
        private readonly LogInManager _logInManager;

        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<KhachHang, long> _khachHangRepository;
        private readonly SendEmail _sendEmail;

        private readonly IRepository<DanhMucNhomDichVu> _nhomDichVuRepository;
        private readonly IRepository<NhanVienNhomDichVu, long> _nhanVienNhomDichVuRepository;
        private readonly IRepository<DanhMucTinhThanh> _tinhThanhRepository;
        private readonly IRepository<NhanVienTinhThanh, long> _nhanVienTinhThanhRepository;
        private readonly IRepository<DanhMucQuanHuyen> _quanHuyenRepository;
        private readonly IRepository<NhanVienQuanHuyen, long> _nhanVienQuanHuyenRepository;
        private readonly IRepository<TramDichVu> _tramDichVuRepository;

        public AccountAppService(
            UserRegistrationManager userRegistrationManager,
            UserManager userManager,
            IPasswordHasher<User> passwordHasher,
            IAbpSession abpSession,
            LogInManager logInManager,
            IRepository<User, long> userRepository,
            IRepository<KhachHang, long> khachHangRepository,
            SendEmail sendEmail,
            IRepository<DanhMucNhomDichVu> nhomDichVuRepository,
            IRepository<NhanVienNhomDichVu, long> nhanVienNhomDichVuRepository,
            IRepository<DanhMucTinhThanh> tinhThanhRepository,
            IRepository<NhanVienTinhThanh, long> nhanVienTinhThanhRepository,
            IRepository<DanhMucQuanHuyen> quanHuyenRepository,
            IRepository<NhanVienQuanHuyen, long> nhanVienQuanHuyenRepository,
            IRepository<TramDichVu> tramDichVuRepository)
        {
            _userRegistrationManager = userRegistrationManager;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _abpSession = abpSession;
            _logInManager = logInManager;

            _userRepository = userRepository;
            _khachHangRepository = khachHangRepository;
            _sendEmail = sendEmail;

            _nhomDichVuRepository = nhomDichVuRepository;
            _nhanVienNhomDichVuRepository = nhanVienNhomDichVuRepository;
            _tinhThanhRepository = tinhThanhRepository;
            _nhanVienTinhThanhRepository = nhanVienTinhThanhRepository;
            _quanHuyenRepository = quanHuyenRepository;
            _nhanVienQuanHuyenRepository = nhanVienQuanHuyenRepository;
            _tramDichVuRepository = tramDichVuRepository;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            string ActiveCode = GlobalFunction.RandomNumber(100001, 999999).ToString();

            var user = await _userRegistrationManager.RegisterAsync(
                            input.PhoneNumber,
                            input.EmailAddress,
                            ActiveCode,
                            input.Password,
                            false,          // Not active by default
                            true
                        );

            // Kiểm tra bên bảng KhachHang, nếu có bản ghi trùng SĐT mà chưa kết nối vào tài khoản nào thì tiến hành kết nối

            KhachHang newKhachHang = await _khachHangRepository.FirstOrDefaultAsync(w => w.PhoneNumber == input.PhoneNumber && w.UserId == null);
            if (newKhachHang == null)
            {
                newKhachHang = new KhachHang
                {
                    PhoneNumber = input.PhoneNumber,
                    EmailAddress = input.EmailAddress,
                    UserId = user.Id
                };
                await _khachHangRepository.InsertAsync(newKhachHang);
            }
            else
            {
                newKhachHang.EmailAddress = input.EmailAddress;
                newKhachHang.UserId = user.Id;

                //await CurrentUnitOfWork.SaveChangesAsync();
            }

            await SendCode(input.EmailAddress, ActiveCode);

            //var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                //CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
                CanLogin = false,
                Message = L("UserIsNotActive")
            };
        }

        [AbpAuthorize]
        public async Task<bool> ChangePassword(ChangePasswordDto input)
        {
            long userId = _abpSession.UserId.Value;
            var user = await _userManager.GetUserByIdAsync(userId);
            var loginAsync = await _logInManager.LoginAsync(user.UserName, input.CurrentPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException(L("WrongCurrentPassword"));
            }

            user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
            CurrentUnitOfWork.SaveChanges();
            return true;
        }

        public async Task<string> ResetPassword(ResetPasswordInput input)
        {
            string RandomPassword = GlobalFunction.MakeRandomPassword();

            var currentUser = await _userRepository.FirstOrDefaultAsync(w => w.UserName == input.UserName && w.EmailAddress == input.EmailAddress);

            if (currentUser != null)
            {
                if (!currentUser.IsActive)
                    throw new UserFriendlyException(L("UserIsNotActiveOrLocked"));

                currentUser.Password = _passwordHasher.HashPassword(currentUser, RandomPassword);
                CurrentUnitOfWork.SaveChanges();

                // Gửi mail cho User
                try
                {
                    await _sendEmail.ResetPasswordEmail(input.EmailAddress, RandomPassword);
                }
                catch (Exception ex)
                {
                    Logger.Error("Lỗi gửi mail reset mật khẩu", ex);

                    throw new UserFriendlyException(L("SendEmailFailed"), ex.Message);
                }
                return L("SendEmailSuccessfully");
            }
            else
            {
                throw new UserFriendlyException(L("WrongUserNameOrEmail"));
            }
        }

        [AbpAuthorize]
        public async Task<GetInfoDto> GetInfo()
        {
            long userId = _abpSession.UserId.Value;
            //var user = await _userManager.GetUserByIdAsync(userId);

            var UserData = await (from User in _userRepository.GetAll().Where(w => w.Id == userId)

                                  join KhachHang in _khachHangRepository.GetAll() on User.Id equals KhachHang.UserId into J1
                                  from KhachHang in J1.DefaultIfEmpty()

                                  select new
                                  {
                                      User,
                                      KhachHang.Address,
                                      KhachHang.DistrictId,
                                      KhachHang.ProvinceId
                                  }).FirstOrDefaultAsync();

            if (UserData == null)
                throw new UserFriendlyException(L("UserIsNotLogin"));

            GetInfoDto result = ObjectMapper.Map<GetInfoDto>(UserData.User);

            result.ProfilePicture = UserData.User.ProfilePicture != null ? Convert.ToBase64String(UserData.User.ProfilePicture) : "";
            result.Address = UserData.Address;
            result.DistrictId = UserData.DistrictId;
            result.ProvinceId = UserData.ProvinceId;

            List<string> NhomDichVu = new List<string>();
            string PhuTrach = "";
            if (!UserData.User.IsCustomer)
            {
                NhomDichVu = await (from DmNhomDichVu in _nhomDichVuRepository.GetAll()
                                    join NvNhomDichVu in _nhanVienNhomDichVuRepository.GetAll() on DmNhomDichVu.Id equals NvNhomDichVu.NhomDichVuId
                                    where NvNhomDichVu.NhanVienId == UserData.User.Id
                                    select DmNhomDichVu.Name).ToListAsync();


                var NvQuanHuyen = _nhanVienQuanHuyenRepository.GetAll().Where(w => w.NhanVienId == UserData.User.Id).Select(s => s.QuanHuyenId);
                var TinhThanhQuanHuyen = await (from NvTinhThanh in _nhanVienTinhThanhRepository.GetAll().Where(w => w.NhanVienId == UserData.User.Id)
                                                join DmTinhThanh in _tinhThanhRepository.GetAll() on NvTinhThanh.TinhThanhId equals DmTinhThanh.Id

                                                join DmQuanHuyen in _quanHuyenRepository.GetAll().Where(w => NvQuanHuyen.Contains(w.Id))
                                                    on DmTinhThanh.Id equals DmQuanHuyen.TinhThanhId into J1
                                                from DmQuanHuyen in J1.DefaultIfEmpty()

                                                select new
                                                {
                                                    TinhThanhName = DmTinhThanh.Name,
                                                    QuanHuyenName = DmQuanHuyen.Name
                                                } into L1

                                                group L1.QuanHuyenName by L1.TinhThanhName into G1
                                                select new
                                                {
                                                    TinhThanhName = G1.Key,
                                                    QuanHuyenList = G1.Where(w => w != null).ToList()
                                                }).ToListAsync();

                foreach (var item in TinhThanhQuanHuyen)
                {
                    if (string.IsNullOrWhiteSpace(PhuTrach))
                        PhuTrach = item.TinhThanhName;
                    else
                        PhuTrach += string.Format("; {0}", item.TinhThanhName);

                    PhuTrach += item.QuanHuyenList.Count > 0 ? ": " + string.Join(", ", item.QuanHuyenList) : "";
                }

                if (UserData.User.TramDichVuId > 0)
                {
                    result.TramDichVu = _tramDichVuRepository.GetAll().Where(w => w.Id == (int)UserData.User.TramDichVuId).Select(s => s.Name).FirstOrDefault();
                }
            }

            result.ChuyenMon = NhomDichVu.Count > 0 ? string.Join("; ", NhomDichVu) : "";
            result.PhuTrach = PhuTrach;

            return result;
        }

        [AbpAuthorize]
        [HttpPost]
        public async Task<string> UpdateInfo(UpdateInfoDto input)
        {
            long userId = _abpSession.UserId.Value;
            var user = await _userManager.GetUserByIdAsync(userId);

            KhachHang khachhang = null;
            bool NeedInsertKhachHang = false;

            if (user.IsCustomer)
            {
                khachhang = await _khachHangRepository.FirstOrDefaultAsync(w => w.UserId == user.Id);
                if (khachhang == null)
                {
                    NeedInsertKhachHang = true;

                    khachhang = new KhachHang
                    {
                        PhoneNumber = user.PhoneNumber,
                        EmailAddress = user.EmailAddress,
                        UserId = user.Id
                    };
                }
            }

            if (!string.IsNullOrWhiteSpace(input.ProfilePicture))
            {
                user.ProfilePicture = Convert.FromBase64String(input.ProfilePicture);
            }

            if (!string.IsNullOrWhiteSpace(input.Name))
            {
                user.Name = input.Name;

                if (khachhang != null)
                    khachhang.Name = input.Name;
            }
            if (!string.IsNullOrWhiteSpace(input.PhoneNumber))
            {
                // Chỉ cho phép cập nhật số điện thoại cho nhân viên
                if (!user.IsCustomer)
                    user.PhoneNumber = input.PhoneNumber;
            }
            if (!string.IsNullOrWhiteSpace(input.EmailAddress))
            {
                user.EmailAddress = input.EmailAddress;

                if (khachhang != null)
                    khachhang.EmailAddress = input.EmailAddress;
            }
            if (input.Birthday.HasValue)
            {
                user.Birthday = input.Birthday;

                if (khachhang != null)
                    khachhang.Birthday = input.Birthday;
            }
            if (!string.IsNullOrWhiteSpace(input.Description))
            {
                user.Description = input.Description;

                if (khachhang != null)
                    khachhang.Description = input.Description;
            }

            if (khachhang != null)
            {
                if (!string.IsNullOrWhiteSpace(input.Address))
                {
                    khachhang.Address = input.Address;
                }
                if (input.DistrictId > 0)
                {
                    khachhang.DistrictId = input.DistrictId;
                }
                if (input.ProvinceId > 0)
                {
                    khachhang.ProvinceId = input.ProvinceId;
                }
            }

            if (NeedInsertKhachHang)
            {
                try
                {
                    await _khachHangRepository.InsertAsync(khachhang);
                }
                catch (Exception ex)
                {
                    Logger.Error("Không tạo được bản ghi khách hàng", ex);

                    throw new UserFriendlyException("Không tạo được bản ghi khách hàng", ex.Message);
                }
            }
            CurrentUnitOfWork.SaveChanges();

            return await Task.FromResult<string>("OK");
        }

        public async Task<string> SendActiveCode(SendActiveCodeInput input)
        {
            var currentUser = await _userRepository.FirstOrDefaultAsync(w => w.UserName == input.UserName && w.EmailAddress == input.EmailAddress);

            if (currentUser != null)
            {
                if (currentUser.IsActive)
                    throw new UserFriendlyException(L("UserIsActivated"));

                if (string.IsNullOrWhiteSpace(currentUser.EmailConfirmationCode))
                    throw new UserFriendlyException(L("UserIsLocked"));

                string ActiveCode = GlobalFunction.RandomNumber(100001, 999999).ToString();

                currentUser.EmailConfirmationCode = ActiveCode;
                CurrentUnitOfWork.SaveChanges();
                
                // Gửi mail cho User
                if (await SendCode(input.EmailAddress, ActiveCode))
                    return L("SendEmailSuccessfully");
                else
                    return L("SendEmailFailed");
            }
            else
            {
                throw new UserFriendlyException(L("WrongUserNameOrEmail"));
            }
        }

        public async Task<string> ActiveAccount(ActiveAccountInput input)
        {
            var currentUser = await _userRepository.FirstOrDefaultAsync(w => w.UserName == input.UserName);

            if (currentUser != null)
            {
                if (currentUser.IsActive)
                    throw new UserFriendlyException(L("UserIsActivated"));

                if (string.IsNullOrWhiteSpace(currentUser.EmailConfirmationCode))
                    throw new UserFriendlyException(L("UserIsLocked"));

                if (currentUser.EmailConfirmationCode != input.ActiveCode)
                    throw new UserFriendlyException(L("WrongActiveCode"));

                currentUser.IsActive = true;
                currentUser.EmailConfirmationCode = null;
                CurrentUnitOfWork.SaveChanges();

                return L("ActivatedSuccessfully");
            }
            else
            {
                throw new UserFriendlyException(L("WrongUserName"));
            }
        }

        private async Task<bool> SendCode(string EmailAddress, string ActiveCode)
        {
            // Gửi mã kích hoạt tài khoản qua Email
            try
            {
                await _sendEmail.ActiveCodeEmail(EmailAddress, ActiveCode);
            }
            catch (Exception ex)
            {
                Logger.Error("Lỗi gửi mail active", ex);

                return false;
            }

            return true;
        }
    }
}
