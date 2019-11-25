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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewCM.Authorization;
using NewCM.DanhMuc.DichVus.Dto;
using NewCM.DbEntities;
using NewCM.Global;

namespace NewCM.DanhMuc.DichVus
{
    [AbpAuthorize(PermissionNames.Pages_DanhMucDichVu), DisableAuditing]
    public class DanhMucDichVuAppService : AsyncCrudAppService<DanhMucDichVu, DanhMucDichVuDto, int, GetAllDanhMucDichVuInput, CreateDanhMucDichVuDto, DanhMucDichVuDto>, IDanhMucDichVuAppService
    {
        private readonly IRepository<DanhMucDichVu> _danhMucDichVuRepository;
        private readonly IRepository<DanhMucNhomDichVu> _danhMucNhomDichVuRepository;

        public DanhMucDichVuAppService(
            IRepository<DanhMucDichVu> danhMucDichVuRepository,
            IRepository<DanhMucNhomDichVu> danhMucNhomDichVuRepository) : base(danhMucDichVuRepository)
        {
            _danhMucDichVuRepository = danhMucDichVuRepository;
            _danhMucNhomDichVuRepository = danhMucNhomDichVuRepository;
        }

        public override async Task<PagedResultDto<DanhMucDichVuDto>> GetAll(GetAllDanhMucDichVuInput input)
        {
            string Keyword = GlobalFunction.RegexFormat(input.Keyword);

            var query = from DichVu in _danhMucDichVuRepository.GetAll()
                            .WhereIf(!string.IsNullOrWhiteSpace(Keyword), w => w.Name.Contains(Keyword))
                            .WhereIf(input.NhomDichVuId.HasValue, w => w.NhomDichVuId == input.NhomDichVuId)
                            .WhereIf(input.IsActive.HasValue, w => w.IsActive == input.IsActive)

                        join NhomDichVu in _danhMucNhomDichVuRepository.GetAll() on DichVu.NhomDichVuId equals NhomDichVu.Id into J1
                        from NhomDichVu in J1.DefaultIfEmpty()

                        select new
                        {
                            DichVu,
                            NhomDichVuName = NhomDichVu != null ? NhomDichVu.Name : ""
                        };

            var TotalCount = await query.CountAsync();
            var PagedDichVu = await query.OrderBy(o => o.DichVu.Name).PageBy(input).ToListAsync();

            var DichVus = new List<DanhMucDichVuDto>();
            foreach (var item in PagedDichVu)
            {
                var DichVu = ObjectMapper.Map<DanhMucDichVuDto>(item.DichVu);
                DichVu.NhomDichVuName = item.NhomDichVuName;

                DichVus.Add(DichVu);
            }

            return new PagedResultDto<DanhMucDichVuDto>(
                TotalCount,
                DichVus
            );
        }

        [HttpPost]
        public override Task<DanhMucDichVuDto> Update(DanhMucDichVuDto input)
        {
            return base.Update(input);
        }

        protected override IQueryable<DanhMucDichVu> CreateFilteredQuery(GetAllDanhMucDichVuInput input)
        {
            string Keyword = GlobalFunction.RegexFormat(input.Keyword);

            return base.CreateFilteredQuery(input)
                .WhereIf(!string.IsNullOrWhiteSpace(Keyword), w => w.Name.Contains(Keyword))
                .WhereIf(input.NhomDichVuId.HasValue, w => w.NhomDichVuId == input.NhomDichVuId);
        }

        protected override IQueryable<DanhMucDichVu> ApplySorting(IQueryable<DanhMucDichVu> query, GetAllDanhMucDichVuInput input)
        {
            return base.ApplySorting(query, input)
                .OrderBy(o => o.Name);
        }
    }
}
