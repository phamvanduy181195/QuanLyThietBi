using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewCM.Authorization;
using NewCM.Authorization.Users;
using NewCM.CongViecs.Dto;
using NewCM.CongViecs.Importing;
using NewCM.DbEntities;
using NewCM.Global;
using NewCM.Global.Dto;
using NewCM.Net.MimeTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace NewCM.CongViecs
{
    [AbpAuthorize(PermissionNames.Pages_QuanLyCongViec)]
    public class QuanLyCongViecAppService : AsyncCrudAppService<CongViec, CongViecDto, long, GetAllCongViecInput, CreateCongViecDto, CongViecDto>, IQuanLyCongViecAppService
    {
        private readonly IRepository<CongViec, long> _congViecRepository;
        private readonly IRepository<DanhMucDichVu> _dichVuRepository;
        private readonly IRepository<TramDichVu> _tramDichVuRepository;
        private readonly IRepository<User, long> _nhanVienRepository;
        private readonly IRepository<DanhMucTinhThanh> _tinhThanhRepository;
        private readonly IRepository<DanhMucQuanHuyen> _quanHuyenRepository;
        private readonly IGlobalCache _globalCache;

        private readonly IAppFolders _appFolders;
        private readonly ICongViecExcelImporter _congViecExcelImporter;

        public QuanLyCongViecAppService(
            IRepository<CongViec, long> congViecRepository,
            IRepository<DanhMucDichVu> dichVuRepository,
            IRepository<TramDichVu> tramDichVuRepository,
            IRepository<User, long> nhanVienRepository,
            IRepository<DanhMucTinhThanh> tinhThanhRepository,
            IRepository<DanhMucQuanHuyen> quanHuyenRepository,
            IGlobalCache globalCache,
            IAppFolders appFolders,
            ICongViecExcelImporter congViecExcelImporter) : base(congViecRepository)
        {
            _congViecRepository = congViecRepository;
            _dichVuRepository = dichVuRepository;
            _tramDichVuRepository = tramDichVuRepository;
            _nhanVienRepository = nhanVienRepository;
            _tinhThanhRepository = tinhThanhRepository;
            _quanHuyenRepository = quanHuyenRepository;
            _globalCache = globalCache;

            _appFolders = appFolders;
            _congViecExcelImporter = congViecExcelImporter;

            LocalizationSourceName = NewCMConsts.LocalizationSourceName;
        }
        public override async Task<PagedResultDto<CongViecDto>> GetAll(GetAllCongViecInput input)
        {
            input.Keyword = GlobalFunction.RegexFormat(input.Keyword);

            var query = from CongViec in _congViecRepository.GetAll()
                            .WhereIf(!string.IsNullOrEmpty(input.Keyword), w => w.KhachHangName.Contains(input.Keyword) || w.SoDienThoai.Contains(input.Keyword))
                            .WhereIf(input.TramDichVuId.HasValue, w => w.TramDichVuId == input.TramDichVuId || (input.TramDichVuId == 0 && w.TramDichVuId == null))
                            .WhereIf(input.TrangThaiId.HasValue, w => w.TrangThaiId == input.TrangThaiId)
                        join DichVu in _dichVuRepository.GetAll() on CongViec.DichVuId equals DichVu.Id into J1
                        from DichVu in J1.DefaultIfEmpty()

                        join TramDichVu in _tramDichVuRepository.GetAll() on CongViec.TramDichVuId equals TramDichVu.Id into J5
                        from TramDichVu in J5.DefaultIfEmpty()

                        join NhanVien in _nhanVienRepository.GetAll() on CongViec.NhanVienId equals NhanVien.Id into J2
                        from NhanVien in J2.DefaultIfEmpty()

                        join TinhThanh in _tinhThanhRepository.GetAll() on CongViec.DiaChiTinhThanhId equals TinhThanh.Id into J3
                        from TinhThanh in J3.DefaultIfEmpty()

                        join QuanHuyen in _quanHuyenRepository.GetAll() on CongViec.DiaChiQuanHuyenId equals QuanHuyen.Id into J4
                        from QuanHuyen in J4.DefaultIfEmpty()

                        select new
                        {
                            CongViec,
                            DichVuName = DichVu.Name,
                            TramDichVuName = TramDichVu.Name,
                            NhanVienName = NhanVien.Name,
                            TinhThanhName = TinhThanh.Name,
                            QuanHuyenName = QuanHuyen.Name
                        };
            var TotalCount = await query.CountAsync();
            var PagedCongViec = await query.OrderBy(o => o.CongViec.NgayGioHen).PageBy(input).ToListAsync();

            var CongViecs = new List<CongViecDto>();
            foreach (var item in PagedCongViec)
            {
                var CongViec = ObjectMapper.Map<CongViecDto>(item.CongViec);
                CongViec.DichVuName = item.DichVuName;
                CongViec.TramDichVuName = item.TramDichVuName;
                CongViec.NhanVienName = item.NhanVienName;
                CongViec.DiaChi += string.IsNullOrWhiteSpace(item.QuanHuyenName) ? "" : ", " + item.QuanHuyenName;
                CongViec.DiaChi += string.IsNullOrWhiteSpace(item.TinhThanhName) ? "" : ", " + item.TinhThanhName;
                CongViec.TrangThaiName = item.CongViec.TrangThaiId >= 0 && GlobalModel.TrangThaiCongViec.ContainsKey((int)item.CongViec.TrangThaiId) ? GlobalModel.TrangThaiCongViec[(int)item.CongViec.TrangThaiId] : "";

                CongViecs.Add(CongViec);
            }

            return new PagedResultDto<CongViecDto>
            {
                TotalCount = TotalCount,
                Items = CongViecs
            };
        }

        public override async Task<CongViecDto> Create(CreateCongViecDto input)
        {
            input.TrangThaiId = 0;

            // Ghép chuỗi để có địa chỉ đầy đủ
            string Address = input.DiaChi;
            string QuanHuyenName = await _globalCache.GetQuanHuyenName(input.DiaChiTinhThanhId, input.DiaChiQuanHuyenId);
            Address += string.IsNullOrWhiteSpace(QuanHuyenName) ? "" : ", " + QuanHuyenName;
            string TinhThanhName = await _globalCache.GetTinhThanhName(input.DiaChiTinhThanhId);
            Address += string.IsNullOrWhiteSpace(TinhThanhName) ? "" : ", " + TinhThanhName;
            Address += ", Việt Nam";
            input.Location = await GlobalFunction.GetLongLatFromAddress(_globalCache.GetGoogleApiKey(), Address);

            // Nghiên cứu viết lại
            var CongViec = ObjectMapper.Map<CongViec>(input);
            CongViec.TrangThaiId = 0;
            CongViec.TimeId = string.Format("{0:yyyyMM}", CongViec.CreationTime);
            await _congViecRepository.InsertAsync(CongViec);

            return ObjectMapper.Map<CongViecDto>(CongViec);
            //return await base.Create(input);
        }

        [HttpPost]
        public override async Task<CongViecDto> Update(CongViecDto input)
        {
            var CongViec = await _congViecRepository.FirstOrDefaultAsync(input.Id);

            if (CongViec == null)
                throw new UserFriendlyException(L("CongViecIsNotFound"));

            CongViec.TramDichVuId = input.TramDichVuId;
            CongViec.TieuDe = input.TieuDe;
            CongViec.NoiDung = input.NoiDung;
            CongViec.DichVuId = input.DichVuId;
            CongViec.KhachHangId = input.KhachHangId;
            CongViec.KhachHangName = input.KhachHangName;
            CongViec.SoDienThoai = input.SoDienThoai;
            CongViec.DiaChi = input.DiaChi;
            CongViec.DiaChiTinhThanhId = input.DiaChiTinhThanhId;
            CongViec.DiaChiQuanHuyenId = input.DiaChiQuanHuyenId;

            if (!CongViec.DaCapNhatLocation )
            {
                // Ghép chuỗi để có địa chỉ đầy đủ
                string Address = input.DiaChi;
                string QuanHuyenName = await _globalCache.GetQuanHuyenName(input.DiaChiTinhThanhId, input.DiaChiQuanHuyenId);
                Address += string.IsNullOrWhiteSpace(QuanHuyenName) ? "" : ", " + QuanHuyenName;
                string TinhThanhName = await _globalCache.GetTinhThanhName(input.DiaChiTinhThanhId);
                Address += string.IsNullOrWhiteSpace(TinhThanhName) ? "" : ", " + TinhThanhName;
                Address += ", Việt Nam";

                CongViec.Location = await GlobalFunction.GetLongLatFromAddress(_globalCache.GetGoogleApiKey(), Address);
            }

            return input;
        }

        public override async Task<CongViecDto> Get(EntityDto<long> input)
        {
            return await base.Get(input);
        }

        [AbpAuthorize(PermissionNames.Pages_QuanLyCongViec)]
        public override async Task Delete(EntityDto<long> input)
        {
            await base.Delete(input);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        [AbpAuthorize(PermissionNames.Pages_QuanLyCongViec_PhanBoVeTram)]
        public async Task PhanBoVeTram(PhanBoVeTramInput input)
        {
            // Kiểm tra nếu tài khoản có Role là Admin thì mới cho phép thực hiện
            
            var CongViecs = await _congViecRepository.GetAll().Where(w => input.CongViecIds.Contains(w.Id)).ToListAsync();
            foreach (var item in CongViecs)
            {
                item.TramDichVuId = input.TramDichVuId;
                item.GhiChuQuanLy = input.GhiChuQuanLy;
            }
        }

        [AbpAuthorize(PermissionNames.Pages_QuanLyCongViec_PhanBoVeNhanVien)]
        public async Task PhanBoVeNhanVien(PhanBoVeNhanVienInput input)
        {
            // Kiểm tra nếu tài khoản có Role là Trạm Trưởng thì mới cho phép thực hiện

            var CongViecs = await _congViecRepository.GetAll().Where(w => input.CongViecIds.Contains(w.Id)).ToListAsync();
            foreach (var item in CongViecs)
            {
                item.NhanVienId = input.NhanVienId;
                item.GhiChuQuanLy = input.GhiChuQuanLy;
                item.TrangThaiId = (int)GlobalConst.TrangThaiCongViec.DaPhanBo;

                if (!item.TramDichVuId.HasValue)
                    item.TramDichVuId = input.TramDichVuId;
            }
        }

        public async Task ThuHoiCongViec(ThuHoiCongViecInput input)
        {
            // Kiểm tra nếu tài khoản có Role là Trạm Trưởng thì mới cho phép thực hiện

            var CongViec = await _congViecRepository.FirstOrDefaultAsync(input.CongViecId);
            
            if (CongViec != null)
            {
                CongViec.NhanVienId = null;

                // Nếu trạng thái đang ở đã phân bổ hoặc đã nhận => Chờ phân bổ
                // Nếu trạng thái đang ở giai đoạn thực hiện => giữ nguyên
                if (CongViec.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.DaPhanBo
                    || CongViec.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.DaNhan)
                    CongViec.TrangThaiId = (int)GlobalConst.TrangThaiCongViec.ChoPhanBo;

                CongViec.GhiChuQuanLy = input.GhiChuQuanLy;

                // Ghi lịch sử thu hồi
            }
        }

        public async Task HuyBoCongViec(HuyBoCongViecInput input)
        {
            // Kiểm tra nếu tài khoản có Role là Trạm Trưởng thì mới cho phép thực hiện

            var CongViec = await _congViecRepository.FirstOrDefaultAsync(input.CongViecId);

            if (CongViec != null)
            {
                CongViec.TrangThaiId = (int)GlobalConst.TrangThaiCongViec.BiTuChoi;

                CongViec.GhiChuQuanLy = input.GhiChuQuanLy;

                // Ghi lịch sử thu hồi
            }
        }

        public async Task XacNhanLinhKien(EntityDto<long> input)
        {
            var CongViec = await _congViecRepository.FirstOrDefaultAsync(w => w.Id == input.Id && w.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.ChoLinhKien);

            if (CongViec == null)
                throw new UserFriendlyException(L("CongViecIsNotFound"));

            CongViec.TrangThaiId = (int)GlobalConst.TrangThaiCongViec.DangXuLy;
        }

        public async Task XacNhanHoanThanh(EntityDto<long> input)
        {
            var CongViec = await _congViecRepository.FirstOrDefaultAsync(w => w.Id == input.Id && w.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.ChoXacNhanHoanThanh);

            if (CongViec == null)
                throw new UserFriendlyException(L("CongViecIsNotFound"));

            CongViec.TrangThaiId = (int)GlobalConst.TrangThaiCongViec.DaHoanThanh;
        }

        public async Task<int> GetTramIdMacDinh()
        {
            var query = await _tramDichVuRepository.FirstOrDefaultAsync(w => w.TramTruongId == AbpSession.UserId);

            return query != null ? query.Id : 0;
        }

        public async Task<string> ImportFileExcel(string FileName)
        {
            string ReturnMessage = "Kết quả nhập file:";

            string FilePath = Path.Combine(_appFolders.CongViecImportFolder, FileName);

            if (!File.Exists(FilePath))
            {
                return "Có lỗi: Không upload được file lên server!";
            }

            var ReadResult = await _congViecExcelImporter.ReadFromExcel(FilePath);

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
                foreach (var congviec in ReadResult.ListResult)
                {
                    await _congViecRepository.InsertAsync(congviec);
                }
            }

            try
            {
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                string Err = ex.Message;
            }

            return ReturnMessage;
        }

        public async Task<FileDto> DownloadFileMau()
        {
            string FileName = string.Format("ImportCongViec.xlsx");

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

        public async Task<DuplicateThongTinSampleOutput> GetTotalDuplicateFile(DuplicateThongTinSampleInput input)
        {
            var ouput = new DuplicateThongTinSampleOutput
            {
                ListFileNameDuplicate = new List<string>(),
                Total = 0
            };

            if (input.ListNameFileUpload != null)
            {
                List<string> lowerFileNameList = input.ListNameFileUpload.Select(s => s.ToLower()).ToList();
                string ForlderPath = GetFilePath(input.Id ?? 0);
                if (Directory.Exists(ForlderPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(ForlderPath);
                    var DuplicateFileList = await directoryInfo.GetFiles().Where(w => lowerFileNameList.Contains(w.Name.ToLower())).ToDynamicListAsync();
                    var fileListDuplicate = directoryInfo.GetFiles().Where(w => lowerFileNameList.Contains(w.Name.ToLower())).Select(s => s.Name).ToList();
                    ouput.Total = DuplicateFileList != null ? DuplicateFileList.Count() : 0;
                    ouput.ListFileNameDuplicate = fileListDuplicate;
                }
            }
            return ouput;
        }

        private string GetFilePath(int Id)
        {
            return $@"{_appFolders.CongViecFileFolder}\{Id}";
        }
    }
}
