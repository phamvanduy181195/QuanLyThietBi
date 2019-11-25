using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewCM.Authorization.Users;
using NewCM.CongViecs.Dto;
using NewCM.DbEntities;
using NewCM.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace NewCM.CongViecs
{
    [AbpAuthorize]
    public class CongViecAppService : NewCMAppServiceBase, ICongViecAppService
    {
        private readonly IRepository<CongViec, long> _congViecRepository;
        private readonly IRepository<CongViecHangMuc, long> _congViecHangMucRepository;
        private readonly IRepository<DanhMucNhomDichVu> _nhomDichVuRepository;
        private readonly IRepository<DanhMucDichVu> _dichVuRepository;
        private readonly IRepository<TramDichVu> _tramDichVuRepository;
        private readonly IRepository<User, long> _nhanVienRepository;
        private readonly IRepository<DanhMucTinhThanh> _tinhThanhRepository;
        private readonly IRepository<DanhMucQuanHuyen> _quanHuyenRepository;
        private readonly IRepository<DanhMucHangMuc> _hangMucRepository;
        private readonly IGlobalCache _globalCache;
        private readonly IAppFolders _appFolders;


        public CongViecAppService(
            IRepository<CongViec, long> congViecRepository,
            IRepository<CongViecHangMuc, long> congViecHangMucRepository,
            IRepository<DanhMucNhomDichVu> nhomDichVuRepository,
            IRepository<DanhMucDichVu> dichVuRepository,
            IRepository<TramDichVu> tramDichVuRepository,
            IRepository<User, long> nhanVienRepository,
            IRepository<DanhMucTinhThanh> tinhThanhRepository,
            IRepository<DanhMucQuanHuyen> quanHuyenRepository,
            IRepository<DanhMucHangMuc> hangMucRepository,
            IGlobalCache globalCache)
        {
            _congViecRepository = congViecRepository;
            _congViecHangMucRepository = congViecHangMucRepository;
            _nhomDichVuRepository = nhomDichVuRepository;
            _dichVuRepository = dichVuRepository;
            _tramDichVuRepository = tramDichVuRepository;
            _nhanVienRepository = nhanVienRepository;
            _tinhThanhRepository = tinhThanhRepository;
            _quanHuyenRepository = quanHuyenRepository;
            _hangMucRepository = hangMucRepository;
            _globalCache = globalCache;
        }

        [HttpPost]
        public async Task<List<CongViecDto>> GetAll(DanhSachCongViecInput input)
        {
            var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);

            if (UserCache.Id == 0)
                return new List<CongViecDto>();

            var query = from CongViec in _congViecRepository.GetAll()
                            .WhereIf(input.TrangThaiIds.Length > 0, w => input.TrangThaiIds.ToList().Contains(w.TrangThaiId))
                            .WhereIf(UserCache.IsCustomer, w => w.KhachHangId == UserCache.CustomerId)
                            .WhereIf(!UserCache.IsCustomer, w => w.NhanVienId == AbpSession.UserId)
                        join DichVu in _dichVuRepository.GetAll() on CongViec.DichVuId equals DichVu.Id into J1
                        from DichVu in J1.DefaultIfEmpty()
                        join NhomDichVu in _nhomDichVuRepository.GetAll() on DichVu.NhomDichVuId equals NhomDichVu.Id into J6
                        from NhomDichVu in J6.DefaultIfEmpty()

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
                            NhomDichVuId = NhomDichVu != null ? NhomDichVu.Id : 0,
                            NhomDichVuName = NhomDichVu.Name,
                            DichVuName = DichVu.Name,
                            TramDichVuName = TramDichVu.Name,
                            NhanVienName = NhanVien.Name,
                            NhanVienPhone = NhanVien.PhoneNumber,
                            TinhThanhName = TinhThanh.Name,
                            QuanHuyenName = QuanHuyen.Name
                        };

            var CongViecs = await query.OrderBy(o => o.CongViec.NgayGioHen).Skip((input.Page - 1) * 10).Take(10).ToListAsync();

            var CongViecDtos = new List<CongViecDto>();
            foreach (var item in CongViecs)
            {
                var CongViec = ObjectMapper.Map<CongViecDto>(item.CongViec);

                if (item.NhomDichVuId > 0)
                    CongViec.NhomDichVuId = item.NhomDichVuId;
                CongViec.NhomDichVuName = item.NhomDichVuName;
                CongViec.DichVuName = item.DichVuName;
                CongViec.TramDichVuName = item.TramDichVuName;
                CongViec.NhanVienName = item.NhanVienName;
                CongViec.NhanVienPhone = item.NhanVienPhone;
                CongViec.DiaChi += string.IsNullOrWhiteSpace(item.QuanHuyenName) ? "" : ", " + item.QuanHuyenName;
                CongViec.DiaChi += string.IsNullOrWhiteSpace(item.TinhThanhName) ? "" : ", " + item.TinhThanhName;
                CongViec.TrangThaiName = item.CongViec.TrangThaiId >= 0 && GlobalModel.TrangThaiCongViec.ContainsKey((int)item.CongViec.TrangThaiId) ? GlobalModel.TrangThaiCongViec[(int)item.CongViec.TrangThaiId] : "";

                CongViec.Image1 = string.IsNullOrWhiteSpace(CongViec.Image1) ? "" : string.Format("/Common/CongViecs/{0}/{1}", item.CongViec.TimeId, CongViec.Image1);
                CongViec.Image2 = string.IsNullOrWhiteSpace(CongViec.Image2) ? "" : string.Format("/Common/CongViecs/{0}/{1}", item.CongViec.TimeId, CongViec.Image2);
                CongViec.Image3 = string.IsNullOrWhiteSpace(CongViec.Image3) ? "" : string.Format("/Common/CongViecs/{0}/{1}", item.CongViec.TimeId, CongViec.Image3);
                CongViec.ImageHoanThanh1 = string.IsNullOrWhiteSpace(CongViec.ImageHoanThanh1) ? "" : string.Format("/Common/CongViecs/{0}/{1}", item.CongViec.TimeId, CongViec.ImageHoanThanh1);
                CongViec.ImageHoanThanh2 = string.IsNullOrWhiteSpace(CongViec.ImageHoanThanh2) ? "" : string.Format("/Common/CongViecs/{0}/{1}", item.CongViec.TimeId, CongViec.ImageHoanThanh2);
                CongViec.ImageHoanThanh3 = string.IsNullOrWhiteSpace(CongViec.ImageHoanThanh3) ? "" : string.Format("/Common/CongViecs/{0}/{1}", item.CongViec.TimeId, CongViec.ImageHoanThanh3);
                CongViec.ImageKhachHang1 = string.IsNullOrWhiteSpace(CongViec.ImageKhachHang1) ? "" : string.Format("/Common/CongViecs/{0}/{1}", item.CongViec.TimeId, CongViec.ImageKhachHang1);
                CongViec.ImageKhachHang2 = string.IsNullOrWhiteSpace(CongViec.ImageKhachHang2) ? "" : string.Format("/Common/CongViecs/{0}/{1}", item.CongViec.TimeId, CongViec.ImageKhachHang2);
                CongViec.ImageKhachHang3 = string.IsNullOrWhiteSpace(CongViec.ImageKhachHang3) ? "" : string.Format("/Common/CongViecs/{0}/{1}", item.CongViec.TimeId, CongViec.ImageKhachHang3);

                CongViecDtos.Add(CongViec);
            }

            return CongViecDtos;
        }

        public async Task<CongViecDto> Get(long Id)
        {
            var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);

            if (UserCache.Id == 0)
                throw new UserFriendlyException(L("UserIsNotLogin"));

            var query = await (from CongViec in _congViecRepository.GetAll().Where(w => w.Id == Id)
                                    .WhereIf(UserCache.IsCustomer, w => w.KhachHangId == UserCache.CustomerId)
                                    .WhereIf(!UserCache.IsCustomer, w => w.NhanVienId == AbpSession.UserId)

                               join DichVu in _dichVuRepository.GetAll() on CongViec.DichVuId equals DichVu.Id into J1
                               from DichVu in J1.DefaultIfEmpty()
                               join NhomDichVu in _nhomDichVuRepository.GetAll() on DichVu.NhomDichVuId equals NhomDichVu.Id into J6
                               from NhomDichVu in J6.DefaultIfEmpty()

                               join TramDichVu in _tramDichVuRepository.GetAll() on CongViec.TramDichVuId equals TramDichVu.Id into J2
                               from TramDichVu in J2.DefaultIfEmpty()

                               join NhanVien in _nhanVienRepository.GetAll() on CongViec.NhanVienId equals NhanVien.Id into J3
                               from NhanVien in J3.DefaultIfEmpty()

                               join TinhThanh in _tinhThanhRepository.GetAll() on CongViec.DiaChiTinhThanhId equals TinhThanh.Id into J4
                               from TinhThanh in J4.DefaultIfEmpty()

                               join QuanHuyen in _quanHuyenRepository.GetAll() on CongViec.DiaChiQuanHuyenId equals QuanHuyen.Id into J5
                               from QuanHuyen in J5.DefaultIfEmpty()

                               select new
                               {
                                   CongViec,
                                   NhomDichVuId = NhomDichVu != null ? NhomDichVu.Id : 0,
                                   NhomDichVuName = NhomDichVu.Name,
                                   DichVuName = DichVu.Name,
                                   TramDichVuName = TramDichVu.Name,
                                   NhanVienName = NhanVien.Name,
                                   NhanVienPhone = NhanVien.PhoneNumber,
                                   TinhThanhName = TinhThanh.Name,
                                   QuanHuyenName = QuanHuyen.Name
                               }).FirstOrDefaultAsync();

            if (query == null)
                throw new UserFriendlyException(L("CongViecIsNotFound"));

            var result = ObjectMapper.Map<CongViecDto>(query.CongViec);

            if (query.NhomDichVuId > 0)
                result.NhomDichVuId = query.NhomDichVuId;
            result.NhomDichVuName = query.NhomDichVuName;
            result.DichVuName = query.DichVuName;
            result.TramDichVuName = query.TramDichVuName;
            result.NhanVienName = query.NhanVienName;
            result.NhanVienPhone = query.NhanVienPhone;
            result.DiaChi += string.IsNullOrWhiteSpace(query.QuanHuyenName) ? "" : ", " + query.QuanHuyenName;
            result.DiaChi += string.IsNullOrWhiteSpace(query.TinhThanhName) ? "" : ", " + query.TinhThanhName;
            result.TrangThaiName = query.CongViec.TrangThaiId >= 0 && GlobalModel.TrangThaiCongViec.ContainsKey((int)query.CongViec.TrangThaiId) ? GlobalModel.TrangThaiCongViec[(int)query.CongViec.TrangThaiId] : "";

            result.Image1 = string.IsNullOrWhiteSpace(result.Image1) ? "" : string.Format("/Common/CongViecs/{0}/{1}", query.CongViec.TimeId, result.Image1);
            result.Image2 = string.IsNullOrWhiteSpace(result.Image2) ? "" : string.Format("/Common/CongViecs/{0}/{1}", query.CongViec.TimeId, result.Image2);
            result.Image3 = string.IsNullOrWhiteSpace(result.Image3) ? "" : string.Format("/Common/CongViecs/{0}/{1}", query.CongViec.TimeId, result.Image3);
            result.ImageHoanThanh1 = string.IsNullOrWhiteSpace(result.ImageHoanThanh1) ? "" : string.Format("/Common/CongViecs/{0}/{1}", query.CongViec.TimeId, result.ImageHoanThanh1);
            result.ImageHoanThanh2 = string.IsNullOrWhiteSpace(result.ImageHoanThanh2) ? "" : string.Format("/Common/CongViecs/{0}/{1}", query.CongViec.TimeId, result.ImageHoanThanh2);
            result.ImageHoanThanh3 = string.IsNullOrWhiteSpace(result.ImageHoanThanh3) ? "" : string.Format("/Common/CongViecs/{0}/{1}", query.CongViec.TimeId, result.ImageHoanThanh3);
            result.ImageKhachHang1 = string.IsNullOrWhiteSpace(result.ImageKhachHang1) ? "" : string.Format("/Common/CongViecs/{0}/{1}", query.CongViec.TimeId, result.ImageKhachHang1);
            result.ImageKhachHang2 = string.IsNullOrWhiteSpace(result.ImageKhachHang2) ? "" : string.Format("/Common/CongViecs/{0}/{1}", query.CongViec.TimeId, result.ImageKhachHang2);
            result.ImageKhachHang3 = string.IsNullOrWhiteSpace(result.ImageKhachHang3) ? "" : string.Format("/Common/CongViecs/{0}/{1}", query.CongViec.TimeId, result.ImageKhachHang3);

            // Lấy danh sách hạng mục công việc
            result.DanhSachHangMuc = await (from CongViecHangMuc in _congViecHangMucRepository.GetAll().Where(w => w.CongViecId == query.CongViec.Id)
                                            join DanhMucHangMuc in _hangMucRepository.GetAll() on CongViecHangMuc.HangMucId equals DanhMucHangMuc.Id

                                            select new CongViecHangMucDto
                                            {
                                                HangMucId = DanhMucHangMuc.Id,
                                                Name = DanhMucHangMuc.Name,
                                                DonViTinh = DanhMucHangMuc.DonViTinh,
                                                SoLuong = CongViecHangMuc.SoLuong,
                                                DonGia = CongViecHangMuc.DonGia,
                                                ThanhTien = CongViecHangMuc.ThanhTien
                                            }).ToListAsync();

            // Tạm tính thành tiền của công việc bằng cách lấy tổng
            result.ThanhTien = result.PhuPhi ?? 0 + result.DanhSachHangMuc.Sum(s => s.ThanhTien);

            return result;
        }

        public async Task<GetTotalOutput> GetTotal()
        {
            GetTotalOutput result = new GetTotalOutput();

            var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);

            if (UserCache.Id == 0)
                throw new UserFriendlyException(L("UserIsNotLogin"));

            var query = _congViecRepository.GetAll()
                        .Where(w => w.TrangThaiId != (int)GlobalConst.TrangThaiCongViec.KhachHangHuy
                            && w.TrangThaiId != (int)GlobalConst.TrangThaiCongViec.BiTuChoi);

            if (UserCache.IsCustomer)
            {
                List<int> TrangThaiDangXuLy = new List<int>() {
                    (int)GlobalConst.TrangThaiCongViec.ChoPhanBo,
                    (int)GlobalConst.TrangThaiCongViec.DaPhanBo,
                    (int)GlobalConst.TrangThaiCongViec.DaNhan,
                    (int)GlobalConst.TrangThaiCongViec.DangXuLy,
                    (int)GlobalConst.TrangThaiCongViec.ChoLinhKien,
                };

                List<int> TrangThaiDaHoanThanh = new List<int>() {
                    (int)GlobalConst.TrangThaiCongViec.ChoXacNhanHoanThanh,
                    (int)GlobalConst.TrangThaiCongViec.DaHoanThanh,
                };

                // Lấy tất cả công việc của khách hàng đang đăng nhập
                var TotalCongViecByTrangThai = await query.Where(w => w.KhachHangId == UserCache.CustomerId)
                                                    .GroupBy(g => g.TrangThaiId)
                                                    .Select(s => new
                                                    {
                                                        TrangThaiId = s.Key,
                                                        Total = s.Count()
                                                    }).ToListAsync();
                
                result.ChoNhanViec = 0;
                result.DangXuLy = TotalCongViecByTrangThai.Where(w => TrangThaiDangXuLy.Contains(w.TrangThaiId)).Select(s => s.Total).Sum();
                result.DaHoanThanh = TotalCongViecByTrangThai.Where(w => TrangThaiDaHoanThanh.Contains(w.TrangThaiId)).Select(s => s.Total).Sum();
            }
            else
            {
                List<int> TrangThaiChoNhanViec = new List<int>() {
                    (int)GlobalConst.TrangThaiCongViec.DaPhanBo
                };

                List<int> TrangThaiDangXuLy = new List<int>() {
                    (int)GlobalConst.TrangThaiCongViec.DaNhan,
                    (int)GlobalConst.TrangThaiCongViec.DangXuLy,
                    (int)GlobalConst.TrangThaiCongViec.ChoLinhKien,
                    (int)GlobalConst.TrangThaiCongViec.ChoXacNhanHoanThanh
                };

                List<int> TrangThaiDaHoanThanh = new List<int>() {
                    (int)GlobalConst.TrangThaiCongViec.DaHoanThanh
                };

                // Lấy tất cả công việc của nhân viên đang đăng nhập
                var TotalCongViecByTrangThai = await query.Where(w => w.NhanVienId == UserCache.Id)
                                                    .GroupBy(g => g.TrangThaiId)
                                                    .Select(s => new
                                                    {
                                                        TrangThaiId = s.Key,
                                                        Total = s.Count()
                                                    }).ToListAsync();

                result.ChoNhanViec = TotalCongViecByTrangThai.Where(w => TrangThaiChoNhanViec.Contains(w.TrangThaiId)).Select(s => s.Total).Sum();
                result.DangXuLy = TotalCongViecByTrangThai.Where(w => TrangThaiDangXuLy.Contains(w.TrangThaiId)).Select(s => s.Total).Sum();
                result.DaHoanThanh = TotalCongViecByTrangThai.Where(w => TrangThaiDaHoanThanh.Contains(w.TrangThaiId)).Select(s => s.Total).Sum();
            }

            return result;
        }

        public async Task<string> Create(CreateYeuCauDto input)
        {
            var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);

            if (UserCache.Id == 0 || !UserCache.IsCustomer)
                throw new UserFriendlyException(L("UserIsNotCustomer"));

            // Ghép chuỗi để có địa chỉ đầy đủ
            string Address = input.DiaChi;
            string QuanHuyenName = await _globalCache.GetQuanHuyenName(input.DiaChiTinhThanhId, input.DiaChiQuanHuyenId);
            Address += string.IsNullOrWhiteSpace(QuanHuyenName) ? "" : ", " + QuanHuyenName;
            string TinhThanhName = await _globalCache.GetTinhThanhName(input.DiaChiTinhThanhId);
            Address += string.IsNullOrWhiteSpace(TinhThanhName) ? "" : ", " + TinhThanhName;
            Address += ", Việt Nam";

            var CongViec = ObjectMapper.Map<CongViec>(input);
            CongViec.KhachHangId = UserCache.CustomerId;
            CongViec.KhachHangName = UserCache.Name;
            CongViec.Location = await GlobalFunction.GetLongLatFromAddress(_globalCache.GetGoogleApiKey(), Address);
            CongViec.TrangThaiId = 0;
            CongViec.NgayYeuCau = DateTime.Now;
            CongViec.TimeId = string.Format("{0:yyyyMM}", CongViec.NgayYeuCau);

            // Tự động phân bổ công việc về Trạm
            var CongViecTheoTram = _congViecRepository.GetAll().Where(w => w.TramDichVuId > 0
                                && (w.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.ChoPhanBo
                                || w.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.DaPhanBo
                                || w.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.DaNhan
                                || w.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.DangXuLy
                                || w.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.ChoLinhKien)
                            ).GroupBy(g => g.TramDichVuId)
                            .Select(s => new { TramDichVuId = s.Key, SoCongViec = s.Count() });

            var query = from TramDichVu in _tramDichVuRepository.GetAll().Where(w => w.DiaChiTinhThanhId == input.DiaChiTinhThanhId || w.DiaChiQuanHuyenId == input.DiaChiQuanHuyenId)

                        join CongViecTungTram in CongViecTheoTram on TramDichVu.Id equals CongViecTungTram.TramDichVuId into J1
                        from CongViecTungTram in J1.DefaultIfEmpty()

                        select new
                        {
                            TramDichVu.Id,
                            TramDichVu.DiaChiTinhThanhId,
                            TramDichVu.DiaChiQuanHuyenId,
                            SoCongViec = CongViecTungTram != null ? CongViecTungTram.SoCongViec : 0
                        };

            var Trams = await query.ToListAsync();

            if (Trams.Count > 0)
            {
                var TramPhanBo = Trams.Where(w => w.DiaChiQuanHuyenId == input.DiaChiQuanHuyenId).OrderBy(o => o.SoCongViec).FirstOrDefault();

                if (TramPhanBo == null)
                    TramPhanBo = Trams.Where(w => w.DiaChiTinhThanhId == input.DiaChiTinhThanhId).OrderBy(o => o.SoCongViec).FirstOrDefault();

                if (TramPhanBo != null)
                {
                    CongViec.TramDichVuId = TramPhanBo.Id;
                }
            }

            await _congViecRepository.InsertAsync(CongViec);

            return "OK";
        }

        //[HttpPost]
        //public async Task<string> Update(UpdateYeuCauDto input)
        //{
        //    var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);

        //    if (UserCache.Id == 0 || UserCache.IsCustomer)
        //        throw new UserFriendlyException(L("UserIsNotStaff"));

        //    var CongViec = await _congViecRepository.FirstOrDefaultAsync(input.Id);
        //    if (CongViec == null)
        //        throw new UserFriendlyException(L("CongViecIsNotFound"));

        //    // Soft delete
        //    _congViecHangMucRepository.Delete(w => w.CongViecId == CongViec.Id && !input.DanhSachHangMuc.Select(s => s.HangMucId).Contains(w.HangMucId));

        //    await _congViecRepository.EnsureCollectionLoadedAsync(CongViec, l => l.CongViecHangMucs);
            
        //    foreach (var item in input.DanhSachHangMuc)
        //    {
        //        var checkHangMuc = CongViec.CongViecHangMucs.Where(w => w.HangMucId == item.HangMucId).FirstOrDefault();
        //        if (checkHangMuc != null)
        //        {
        //            checkHangMuc.DonGia = item.DonGia;
        //            checkHangMuc.SoLuong = item.SoLuong;
        //            checkHangMuc.ThanhTien = item.DonGia * item.SoLuong;
        //        }
        //        else
        //        {
        //            CongViec.CongViecHangMucs.Add(new CongViecHangMuc
        //            {
        //                HangMucId = item.HangMucId,
        //                DonGia = item.DonGia,
        //                SoLuong = item.SoLuong,
        //                ThanhTien = item.DonGia * item.SoLuong
        //            });
        //        }
        //    }

        //    //var DanhSachThem = input.DanhSachHangMuc.Where(w => !CongViec.CongViecHangMucs.Select(s => s.HangMucId).Contains(w.HangMucId)).ToList();
        //    //foreach (var item in DanhSachThem)
        //    //    CongViec.CongViecHangMucs.Add(new CongViecHangMuc
        //    //    {
        //    //        HangMucId = item.HangMucId,
        //    //        DonGia = item.DonGia,
        //    //        SoLuong = item.SoLuong,
        //    //        ThanhTien = item.DonGia * item.SoLuong
        //    //    });

        //    CongViec.PhuPhi = input.PhuPhi;
        //    CongViec.GhiChuNhanVien = input.GhiChuNhanVien;
        //    CongViec.ThanhTien = input.PhuPhi + CongViec.CongViecHangMucs.Sum(s => s.ThanhTien);
        //    CongViec.TrangThaiId = input.TrangThaiId;

        //    return "OK";
        //}

        [HttpPost]
        public async Task<string> NhanViec(NhanViecInput input)
        {
            var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);

            if (UserCache.Id == 0)
                throw new UserFriendlyException(L("UserIsNotLogin"));

            if (UserCache.IsCustomer)
                throw new UserFriendlyException(L("UserIsNotStaff"));

            var CongViec = await _congViecRepository.FirstOrDefaultAsync(w => w.Id == input.Id && w.NhanVienId == UserCache.Id && w.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.DaPhanBo);

            if (CongViec == null)
                throw new UserFriendlyException(L("CongViecIsNotFound"));

            if (input.DongY)
                CongViec.TrangThaiId = (int)Global.GlobalConst.TrangThaiCongViec.DaNhan;
            else
            {
                CongViec.NhanVienId = null;
                CongViec.TrangThaiId = (int)Global.GlobalConst.TrangThaiCongViec.ChoPhanBo;
                CongViec.LyDoKhongNhan = input.LyDo;
            }

            // Lưu lịch sử công việc

            return "OK";
        }

        /// <summary>
        /// Dành cho nhân viên, cập nhật thôn tin Location chính xác khi đến địa điểm khách hàng.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>OK</returns>
        [HttpPost]
        public async Task<string> CapNhatLocation(CapNhatLocationInput input)
        {
            var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);

            if (UserCache.Id == 0)
                throw new UserFriendlyException(L("UserIsNotLogin"));

            if (UserCache.IsCustomer)
                throw new UserFriendlyException(L("UserIsNotStaff"));

            var CongViec = await _congViecRepository.FirstOrDefaultAsync(w => w.Id == input.Id && w.NhanVienId == UserCache.Id
                                && (w.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.DaNhan
                                    || w.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.DangXuLy
                                    || w.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.ChoLinhKien));

            if (CongViec == null)
                throw new UserFriendlyException(L("CongViecIsNotFound"));

            CongViec.Location = string.Format("{0},{1}", input.Lat, input.Lon);
            CongViec.DaCapNhatLocation = true;

            return "OK";
        }

        /// <summary>
        /// Dành cho nhân viên, trả lại công việc về trạm trưởng trong trường hợp không nhận làm tiếp.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>OK</returns>
        [HttpPost]
        public async Task<string> TraVeTramTruong(TraVeTramTruongInput input)
        {
            var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);

            if (UserCache.Id == 0)
                throw new UserFriendlyException(L("UserIsNotLogin"));

            if (UserCache.IsCustomer)
                throw new UserFriendlyException(L("UserIsNotStaff"));

            var CongViec = await _congViecRepository.FirstOrDefaultAsync(w => w.Id == input.Id && w.NhanVienId == UserCache.Id
                            && (w.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.DaNhan || w.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.DangXuLy));

            if (CongViec == null)
                throw new UserFriendlyException(L("CongViecIsNotFound"));

            CongViec.NhanVienId = null;
            CongViec.TrangThaiId = (int)Global.GlobalConst.TrangThaiCongViec.ChoPhanBo;
            CongViec.LyDoTraVe = input.LyDo;

            // Lưu lịch sử công việc

            return "OK";
        }

        [HttpPost]
        public async Task<string> PhanLoaiCongViec(PhanLoaiCongViecInput input)
        {
            var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);

            if (UserCache.Id == 0 || UserCache.IsCustomer)
                throw new UserFriendlyException(L("UserIsNotStaff"));

            var CongViec = await _congViecRepository.FirstOrDefaultAsync(input.Id);
            if (CongViec == null)
                throw new UserFriendlyException(L("CongViecIsNotFound"));

            CongViec.LoaiCongViecId = input.LoaiCongViecId;
            CongViec.DichVuId = input.DichVuId;
            CongViec.GhiChuNhanVien = input.GhiChuNhanVien;

            return "OK";
        }

        [HttpPost]
        public async Task<string> BaoGiaCongViec(BaoGiaCongViecInput input)
        {
            var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);

            if (UserCache.Id == 0 || UserCache.IsCustomer)
                throw new UserFriendlyException(L("UserIsNotStaff"));

            var CongViec = await _congViecRepository.FirstOrDefaultAsync(input.Id);
            if (CongViec == null)
                throw new UserFriendlyException(L("CongViecIsNotFound"));

            // Soft delete
            _congViecHangMucRepository.Delete(w => w.CongViecId == CongViec.Id && !input.DanhSachHangMuc.Select(s => s.HangMucId).Contains(w.HangMucId));

            await _congViecRepository.EnsureCollectionLoadedAsync(CongViec, l => l.CongViecHangMucs);

            foreach (var item in input.DanhSachHangMuc)
            {
                var checkHangMuc = CongViec.CongViecHangMucs.Where(w => w.HangMucId == item.HangMucId).FirstOrDefault();
                if (checkHangMuc != null)
                {
                    checkHangMuc.DonGia = item.DonGia;
                    checkHangMuc.SoLuong = item.SoLuong;
                    checkHangMuc.ThanhTien = item.DonGia * item.SoLuong;
                }
                else
                {
                    CongViec.CongViecHangMucs.Add(new CongViecHangMuc
                    {
                        HangMucId = item.HangMucId,
                        DonGia = item.DonGia,
                        SoLuong = item.SoLuong,
                        ThanhTien = item.DonGia * item.SoLuong
                    });
                }
            }

            if (input.YeuCauLinhKien == 1)
            {
                CongViec.LinhKienThayThe = input.LinhKienThayThe;
                CongViec.LinhKienThanhTien = input.LinhKienThanhTien;

                if (CongViec.TrangThaiId != (int)GlobalConst.TrangThaiCongViec.DangXuLy)
                    CongViec.TrangThaiId = (int)GlobalConst.TrangThaiCongViec.ChoLinhKien;
            }
            else
            {
                CongViec.LinhKienThayThe = null;
                CongViec.LinhKienThanhTien = null;
                CongViec.TrangThaiId = (int)GlobalConst.TrangThaiCongViec.DangXuLy;
            }

            CongViec.PhuPhi = input.PhuPhi;
            CongViec.ThanhTien = input.PhuPhi + CongViec.CongViecHangMucs.Sum(s => s.ThanhTien) + CongViec.LinhKienThanhTien ?? 0;

            return "OK";
        }

        [HttpPost]
        public async Task<string> HoanThanhCongViec(HoanThanhCongViecInput input)
        {
            var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);

            if (UserCache.Id == 0 || UserCache.IsCustomer)
                throw new UserFriendlyException(L("UserIsNotStaff"));

            var CongViec = await _congViecRepository.FirstOrDefaultAsync(input.Id);
            if (CongViec == null)
                throw new UserFriendlyException(L("CongViecIsNotFound"));

            CongViec.GhiChuNhanVien = input.GhiChuNhanVien;
            CongViec.NgayHoanThanh = DateTime.Now;
            CongViec.TrangThaiId = (int)GlobalConst.TrangThaiCongViec.ChoXacNhanHoanThanh;

            return "OK";
        }

        [HttpPost]
        public async Task<string> KhachHangHuy(KhachHangHuyInput input)
        {
            var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);

            if (UserCache.Id == 0)
                throw new UserFriendlyException(L("UserIsNotLogin"));

            if (!UserCache.IsCustomer || UserCache.CustomerId == 0)
                throw new UserFriendlyException(L("UserIsNotCustomer"));

            var CongViec = await _congViecRepository.FirstOrDefaultAsync(w => w.Id == input.Id && w.KhachHangId == UserCache.CustomerId
                            && w.TrangThaiId != (int)GlobalConst.TrangThaiCongViec.DaHoanThanh && w.TrangThaiId != (int)GlobalConst.TrangThaiCongViec.BiTuChoi);

            if (CongViec == null)
                throw new UserFriendlyException(L("CongViecIsNotFound"));

            CongViec.TrangThaiId = (int)Global.GlobalConst.TrangThaiCongViec.KhachHangHuy;
            CongViec.LyDoHuy = input.LyDo;

            // Lưu lịch sử công việc

            return "OK";
        }

        [HttpPost]
        public async Task<string> KhachHangDanhGia(KhachHangDanhGiaInput input)
        {
            var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);

            if (UserCache.Id == 0)
                throw new UserFriendlyException(L("UserIsNotLogin"));

            if (!UserCache.IsCustomer || UserCache.CustomerId == 0)
                throw new UserFriendlyException(L("UserIsNotCustomer"));

            var CongViec = await _congViecRepository.FirstOrDefaultAsync(w => w.Id == input.Id && w.KhachHangId == UserCache.CustomerId && w.DanhGiaDiem == null
                                && (w.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.DaHoanThanh
                                    || w.TrangThaiId == (int)GlobalConst.TrangThaiCongViec.ChoXacNhanHoanThanh));

            if (CongViec == null)
                throw new UserFriendlyException(L("CongViecIsNotFound"));

            CongViec.DanhGiaDiem = input.DanhGiaDiem;
            CongViec.DanhGiaText = input.DanhGiaText;

            return "OK";
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
