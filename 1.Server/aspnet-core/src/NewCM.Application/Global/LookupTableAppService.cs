using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewCM.Authorization.Roles;
using NewCM.Authorization.Users;
using NewCM.DbEntities;
using NewCM.Global.Dto;
using NewCM.KhachHangs.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewCM.Global
{
    [AbpAuthorize, DisableAuditing]
    public class LookupTableAppService : NewCMAppServiceBase
    {
        private readonly IGlobalCache _globalCache;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<TramDichVu> _tramDichVuRepository;
        private readonly IRepository<DanhMucNhomDichVu> _danhMucNhomDichVuRepository;
        private readonly IRepository<DanhMucDichVu> _danhMucDichVuRepository;
        private readonly IRepository<KhachHang, long> _khachHangDichVuRepository;
        private readonly IRepository<DanhMucHangMuc> _danhMucHangMucRepository;

        public LookupTableAppService(IGlobalCache globalCache,
            IRepository<User, long> userRepository,
            IRepository<Role> roleRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<TramDichVu> tramDichVuRepository,
            IRepository<DanhMucNhomDichVu> danhMucNhomDichVuRepository,
            IRepository<DanhMucDichVu> danhMucDichVuRepository,
            IRepository<KhachHang, long> khachHangDichVuRepository,
            IRepository<DanhMucHangMuc> danhMucHangMucRepository)
        {
            _globalCache = globalCache;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _tramDichVuRepository = tramDichVuRepository;
            _danhMucNhomDichVuRepository = danhMucNhomDichVuRepository;
            _danhMucDichVuRepository = danhMucDichVuRepository;
            _khachHangDichVuRepository = khachHangDichVuRepository;
            _danhMucHangMucRepository = danhMucHangMucRepository;
        }

        public async Task<List<LookupTableDto<long>>> GetAllTramTruongForLookupTable(int? TramDichVuId)
        {
            var DanhSachTramTruongId = _tramDichVuRepository.GetAll()
                                       .WhereIf(TramDichVuId.HasValue, w => w.Id != TramDichVuId)
                                       .Select(s => s.TramTruongId)
                                       .Distinct();

            var query = from TramTruong in _userRepository.GetAll().Where(w => w.IsActive && !w.IsCustomer)
                        join VaiTroNguoiDung in _userRoleRepository.GetAll() on TramTruong.Id equals VaiTroNguoiDung.UserId
                        join VaiTro in _roleRepository.GetAll() on VaiTroNguoiDung.RoleId equals VaiTro.Id

                        where VaiTro.Id == (int)GlobalConst.StaticRole.TramTruong
                            && !DanhSachTramTruongId.Contains(TramTruong.Id)

                        select new LookupTableDto<long>
                        {
                            Id = TramTruong.Id,
                            DisplayName = TramTruong.UserName + " - " + TramTruong.Name
                        };

            return await query.ToListAsync();
        }

        public async Task<List<LookupTableDto>> GetAllTinhThanhForLookupTable()
        {
            return await _globalCache.GetTinhThanhCache();

            //var query = _danhMucTinhThanhRepository.GetAll().OrderBy(o => o.Name)
            //            .Select(s => new LookupTableDto
            //            {
            //                Id = s.Id,
            //                DisplayName = s.Name
            //            });

            //return await query.ToListAsync();
        }

        [HttpPost]
        public async Task<List<LookupTableDto>> GetAllQuanHuyenForLookupTable(GetAllQuanHuyenInput input)
        {
            return await _globalCache.GetQuanHuyenCache(input.TinhThanhIds);

            //var query = _danhMucQuanHuyenRepository.GetAll().Where(w => TinhThanhIds.Contains(w.TinhThanhId))
            //            .OrderBy(o => o.TinhThanhId).ThenBy(o => o.Name)
            //            .Select(s => new LookupTableDto
            //            {
            //                Id = s.Id,
            //                DisplayName = s.Name
            //            });

            //return await query.ToListAsync();
        }

        // Lấy toàn bộ nhân viên của Trạm và nhân viên chưa được add vào trạm nào
        public async Task<List<NhanVienLookupTableDto>> GetAllNhanVienForLookupTable(int? TramDichVuId)
        {
            var DanhSachNhanVien = from NhanVien in _userRepository.GetAll()
                                        .Where(w => w.IsActive && !w.IsCustomer)
                                        .Where(w => w.TramDichVuId == null || (TramDichVuId.HasValue && w.TramDichVuId == TramDichVuId))
                                   join VaiTroNguoiDung in _userRoleRepository.GetAll() on NhanVien.Id equals VaiTroNguoiDung.UserId
                                   join VaiTro in _roleRepository.GetAll() on VaiTroNguoiDung.RoleId equals VaiTro.Id
                                   where VaiTro.Id == (int)GlobalConst.StaticRole.NhanVien
                                   orderby NhanVien.UserName
                                   select new NhanVienLookupTableDto
                                   {
                                       Id = NhanVien.Id,
                                       UserName = NhanVien.UserName,
                                       DisplayName = NhanVien.UserName + " - " + NhanVien.Name
                                   };

            return await DanhSachNhanVien.ToListAsync();
        }

        public async Task<List<NhanVienLookupTableDto>> GetAllNhanVienByTramForLookupTable(int TramDichVuId)
        {
            var DanhSachNhanVien = from NhanVien in _userRepository.GetAll()
                                        .Where(w => w.IsActive && !w.IsCustomer)
                                        .Where(w => w.TramDichVuId == TramDichVuId)
                                   orderby NhanVien.UserName
                                   select new NhanVienLookupTableDto
                                   {
                                       Id = NhanVien.Id,
                                       UserName = NhanVien.UserName,
                                       DisplayName = NhanVien.UserName + " - " + NhanVien.Name
                                   };

            return await DanhSachNhanVien.ToListAsync();
        }

        // Lấy toàn bộ khách hàng
        public async Task<List<KhachHangLookupTableDto>> GetAllKhachHangForLookupTable()
        {
            var query = await _khachHangDichVuRepository.GetAll().OrderBy(o => o.PhoneNumber)
                                .Select(s => new KhachHangLookupTableDto
                                {
                                    Id = s.Id,
                                    Name = s.Name,
                                    PhoneNumber = s.PhoneNumber,
                                    Address = s.Address,
                                    ProvinceId = s.ProvinceId,
                                    DistrictId = s.DistrictId
                                }).ToListAsync();

            foreach(var item in query)
            {
                item.DisplayName = string.Format("{0} - {1}", item.PhoneNumber, item.Name);
            }

            return query;
        }

        public async Task<List<LookupTableDto>> GetAllTramDichVuForLookupTable()
        {
            var query = _tramDichVuRepository.GetAll().OrderBy(o => o.Name)
                        .Select(s => new LookupTableDto
                        {
                            Id = s.Id,
                            DisplayName = s.Name
                        });

            return await query.ToListAsync();
        }

        public async Task<List<LookupTableDto>> GetAllNhomDichVuForLookupTable()
        {
            var query = _danhMucNhomDichVuRepository.GetAll().OrderBy(o => o.Name)
                        .Select(s => new LookupTableDto
                        {
                            Id = s.Id,
                            DisplayName = s.Name
                        });

            return await query.ToListAsync();
        }

        public async Task<List<LookupTableDto>> GetAllDichVuForLookupTable(int? NhomDichVuId)
        {
            var query = _danhMucDichVuRepository.GetAll().OrderBy(o => o.Name).Where(w => w.IsActive)
                        .WhereIf(NhomDichVuId.HasValue, w => w.NhomDichVuId == NhomDichVuId)
                        .OrderBy(o => o.Name)
                        .Select(s => new LookupTableDto
                        {
                            Id = s.Id,
                            DisplayName = s.Name
                        });

            return await query.ToListAsync();
        }

        public async Task<List<HangMucLookupTableDto>> GetAllHangMucForLookupTable(int DichVuId)
        {
            var query = _danhMucHangMucRepository.GetAll().Where(w => w.DichVuId == DichVuId).Where(w => w.IsActive)
                        .OrderBy(o => o.Name)
                        .Select(s => new HangMucLookupTableDto
                        {
                            Id = s.Id,
                            DisplayName = s.Name,
                            Name = s.Name,
                            DonGia = s.DonGia,
                            DonViTinh = s.DonViTinh
                        });

            return await query.ToListAsync();
        }

        public async Task<List<LookupTableDto>> GetAllTrangThaiCongViecForLookupTable()
        {
            List<LookupTableDto> query = new List<LookupTableDto>();
            foreach (var item in GlobalModel.TrangThaiCongViec)
            {
                query.Add(new LookupTableDto
                {
                    Id = item.Key,
                    DisplayName = item.Value
                });
            }

            return await Task.FromResult(query);
        }
    }
}
