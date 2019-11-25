using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewCM.Authorization;
using NewCM.DanhMuc.HangMucs.Dto;
using NewCM.DbEntities;
using NewCM.Global;

namespace NewCM.DanhMuc.HangMucs
{
    [AbpAuthorize(PermissionNames.Pages_DanhMucHangMuc), DisableAuditing]
    public class DanhMucHangMucAppService : AsyncCrudAppService<DanhMucHangMuc, DanhMucHangMucDto, int, GetAllDanhMucHangMucInput, CreateDanhMucHangMucDto, DanhMucHangMucDto>, IDanhMucHangMucAppService
    {
        private readonly IRepository<DanhMucHangMuc> _danhMucHangMucRepository;
        private readonly IRepository<DanhMucDichVu> _danhMucDichVuRepository;
        private readonly IRepository<DanhMucNhomDichVu> _danhMucNhomDichVuRepository;

        public DanhMucHangMucAppService(
            IRepository<DanhMucHangMuc> danhMucHangMucRepository,
            IRepository<DanhMucDichVu> danhMucDichVuRepository,
            IRepository<DanhMucNhomDichVu> danhMucNhomDichVuRepository) :base(danhMucHangMucRepository)
        {
            _danhMucHangMucRepository = danhMucHangMucRepository;
            _danhMucDichVuRepository = danhMucDichVuRepository;
            _danhMucNhomDichVuRepository = danhMucNhomDichVuRepository;
        }

        public override async Task<PagedResultDto<DanhMucHangMucDto>> GetAll(GetAllDanhMucHangMucInput input)
        {
            string Keyword = GlobalFunction.RegexFormat(input.Keyword);

            var query = from HangMuc in _danhMucHangMucRepository.GetAll()
                            .WhereIf(!string.IsNullOrWhiteSpace(Keyword), w => w.Name.Contains(Keyword))
                            .WhereIf(input.DichVuId.HasValue, w => w.DichVuId == input.DichVuId)
                            .WhereIf(input.IsActive.HasValue, w => w.IsActive == input.IsActive)

                        join DichVu in _danhMucDichVuRepository.GetAll() on HangMuc.DichVuId equals DichVu.Id into J1
                        from DichVu in J1.DefaultIfEmpty()

                        join NhomDichVu in _danhMucNhomDichVuRepository.GetAll() on DichVu.NhomDichVuId equals NhomDichVu.Id into J2
                        from NhomDichVu in J2.DefaultIfEmpty()

                        where !input.NhomDichVuId.HasValue || NhomDichVu.Id == input.NhomDichVuId

                        select new
                        {
                            HangMuc,
                            DichVuName = DichVu != null ? DichVu.Name : "",
                            NhomDichVuName = NhomDichVu != null ? NhomDichVu.Name : ""
                        };

            var TotalCount = await query.CountAsync();
            var PagedHangMuc = await query.OrderBy(o => o.HangMuc.Name).PageBy(input).ToListAsync();

            List<DanhMucHangMucDto> HangMucs = new List<DanhMucHangMucDto>();
            foreach (var item in PagedHangMuc)
            {
                var HangMuc = ObjectMapper.Map<DanhMucHangMucDto>(item.HangMuc);
                HangMuc.DichVuName = item.DichVuName;
                HangMuc.NhomDichVuName = item.NhomDichVuName;

                HangMucs.Add(HangMuc);
            }

            return new PagedResultDto<DanhMucHangMucDto>
            {
                TotalCount = TotalCount,
                Items = HangMucs
            };
        }

        public override async Task<DanhMucHangMucDto> Get(EntityDto<int> input)
        {
            var query = await (from HangMuc in _danhMucHangMucRepository.GetAll().Where(w => w.Id == input.Id)

                        join DichVu in _danhMucDichVuRepository.GetAll() on HangMuc.DichVuId equals DichVu.Id into J1
                        from DichVu in J1.DefaultIfEmpty()

                        join NhomDichVu in _danhMucNhomDichVuRepository.GetAll() on DichVu.NhomDichVuId equals NhomDichVu.Id into J2
                        from NhomDichVu in J2.DefaultIfEmpty()

                        select new
                        {
                            HangMuc,
                            NhomDichVuId = NhomDichVu != null ? NhomDichVu.Id : 0
                        }).FirstOrDefaultAsync();

            if (query == null)
                throw new UserFriendlyException("Không tìm thấy Hạng Mục!");

            var GetHangMuc = ObjectMapper.Map<DanhMucHangMucDto>(query.HangMuc);
            if (query.NhomDichVuId > 0)
                GetHangMuc.NhomDichVuId = query.NhomDichVuId;

            return GetHangMuc;
        }

        [HttpPost]
        public override Task<DanhMucHangMucDto> Update(DanhMucHangMucDto input)
        {
            return base.Update(input);
        }

        protected override IQueryable<DanhMucHangMuc> CreateFilteredQuery(GetAllDanhMucHangMucInput input)
        {
            string Keyword = GlobalFunction.RegexFormat(input.Keyword);

            return base.CreateFilteredQuery(input)
                .WhereIf(!string.IsNullOrWhiteSpace(Keyword), w => w.Name.Contains(Keyword))
                .WhereIf(input.DichVuId.HasValue, w => w.DichVuId == input.DichVuId);
        }

        protected override IQueryable<DanhMucHangMuc> ApplySorting(IQueryable<DanhMucHangMuc> query, GetAllDanhMucHangMucInput input)
        {
            return base.ApplySorting(query, input)
                .OrderBy(o => o.Name);
        }
    }
}
