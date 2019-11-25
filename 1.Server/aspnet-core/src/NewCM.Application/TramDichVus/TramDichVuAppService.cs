using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewCM.Authorization;
using NewCM.Authorization.Users;
using NewCM.TramDichVus.Dto;
using NewCM.DbEntities;
using System;
using Microsoft.AspNetCore.Mvc;
using NewCM.Global;

namespace NewCM.TramDichVus
{
    [AbpAuthorize(PermissionNames.Pages_TramDichVu), DisableAuditing]
    public class TramDichVuAppService: AsyncCrudAppService<TramDichVu, TramDichVuDto, int, GetAllTramDichVuInput, CreateTramDichVuDto, TramDichVuDto>, ITramDichVuAppService
    {
        private readonly IRepository<TramDichVu> _tramDichVuRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<DanhMucQuanHuyen> _danhMucQuanHuyenRepository;
        private readonly IRepository<DanhMucTinhThanh> _danhMucTinhThanhRepository;

        public TramDichVuAppService(
            IRepository<TramDichVu> tramDichVuRepository,
            IRepository<User, long> userRepository,
            IRepository<DanhMucQuanHuyen> danhMucQuanHuyenRepository,
            IRepository<DanhMucTinhThanh> danhMucTinhThanhRepository) : base(tramDichVuRepository)
        {
            _tramDichVuRepository = tramDichVuRepository;
            _userRepository = userRepository;
            _danhMucQuanHuyenRepository = danhMucQuanHuyenRepository;
            _danhMucTinhThanhRepository = danhMucTinhThanhRepository;

            LocalizationSourceName = NewCMConsts.LocalizationSourceName;
        }

        public async Task<PagedResultDto<GetTramDichVuForView>> GetAllNew(GetAllTramDichVuInput input)
        {
            input.Keyword = GlobalFunction.RegexFormat(input.Keyword);

            var query = from TramDV in _tramDichVuRepository.GetAll()
                            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), w => w.Name.Contains(input.Keyword))
                        join TramTruong in _userRepository.GetAll() on TramDV.TramTruongId equals TramTruong.Id into J1
                        from TramTruong in J1.DefaultIfEmpty()

                        join DmQuanHuyen in _danhMucQuanHuyenRepository.GetAll() on TramDV.DiaChiQuanHuyenId equals DmQuanHuyen.Id into J2
                        from DmQuanHuyen in J2.DefaultIfEmpty()

                        join DmTinhThanh in _danhMucTinhThanhRepository.GetAll() on TramDV.DiaChiTinhThanhId equals DmTinhThanh.Id into J3
                        from DmTinhThanh in J3.DefaultIfEmpty()

                        select new
                        {
                            TramDV,
                            TramTruongName = TramTruong != null ? TramTruong.UserName : "",
                            QuanHuyenName = DmQuanHuyen != null ? ", " + DmQuanHuyen.Name : "",
                            TinhThanhName = DmTinhThanh != null ? ", " + DmTinhThanh.Name : "",
                        };

            var TotalCount = await query.CountAsync();
            var PagedDanhMucTram = await query.OrderBy(o => o.TramDV.Name).PageBy(input).ToListAsync();

            var PagedDanhMucTramForView = PagedDanhMucTram.Select(s => new GetTramDichVuForView
            {
                TramDichVu = ObjectMapper.Map<TramDichVuDto>(s.TramDV),
                TramTruongName = s.TramTruongName,
                DiaChi = s.TramDV.DiaChi + s.QuanHuyenName + s.TinhThanhName,
            }).ToList();

            return new PagedResultDto<GetTramDichVuForView> (
                TotalCount,
                PagedDanhMucTramForView
            );
        }

        public override async Task<TramDichVuDto> Create(CreateTramDichVuDto input)
        {
            var tramDichVuDto = await base.Create(input);

            if (input.UserIds != null)
            {
                CheckErrors(await SetNhanViens(tramDichVuDto.Id, tramDichVuDto.TramTruongId, input.UserIds));
            }

            return tramDichVuDto;
        }

        [HttpPost]
        public override async Task<TramDichVuDto> Update(TramDichVuDto input)
        {
            var tramDichVuDto = await base.Update(input);

            if (input.UserIds != null)
            {
                CheckErrors(await SetNhanViens(tramDichVuDto.Id, tramDichVuDto.TramTruongId, input.UserIds));
            }

            return tramDichVuDto;
        }

        public override async Task Delete(EntityDto<int> input)
        {
            await base.Delete(input);
        }

        protected override TramDichVuDto MapToEntityDto(TramDichVu input)
        {
            var NhanVienId = _userRepository.GetAll().Where(w => w.TramDichVuId == input.Id).Select(s => s.Id);

            var tramDichVuDto = base.MapToEntityDto(input);
            tramDichVuDto.UserIds = NhanVienId.ToArray();
            return tramDichVuDto;
        }

        //protected override IQueryable<TramDichVu> CreateFilteredQuery(GetAllTramDichVuInput input)
        //{
        //    return base.CreateFilteredQuery(input)
        //        .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), t => t.Name.Contains(input.Keyword));
        //}

        //protected override IQueryable<TramDichVu> ApplySorting(IQueryable<TramDichVu> query, GetAllTramDichVuInput input)
        //{
        //    return query.OrderBy(r => r.Name);
        //}

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        private async Task<IdentityResult> SetNhanViens(int TramDichVuId, long TramTruongId, long[] UserIds)
        {
            var DanhSachNhanVien = await _userRepository.GetAll().Where(w => w.TramDichVuId == TramDichVuId).ToListAsync();

            // Xóa nhân viên không được chọn và không phải trạm trưởng
            var DanhSachXoa = DanhSachNhanVien.Where(w => !UserIds.Contains(w.Id) && w.Id != TramTruongId);
            foreach (var NhanVien in DanhSachXoa)
            {
                NhanVien.TramDichVuId = null;
            }

            // Thêm nhân viên mới
            var DanhSachUsernameMoi = UserIds.Where(w => !DanhSachNhanVien.Select(s => s.Id).Contains(w)).ToList();
            var DanhSachMoi = await _userRepository.GetAll().Where(w => DanhSachUsernameMoi.Contains(w.Id) || w.Id == TramTruongId).ToListAsync();
            foreach (var NhanVien in DanhSachMoi)
            {
                NhanVien.TramDichVuId = TramDichVuId;
            }

            try
            {
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = "AddNhanVienError", Description = ex.Message });
            }

            return IdentityResult.Success;
        }
    }
}
