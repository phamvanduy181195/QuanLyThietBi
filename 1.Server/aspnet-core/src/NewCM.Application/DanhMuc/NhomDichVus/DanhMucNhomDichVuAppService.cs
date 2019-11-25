using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using NewCM.Authorization;
using NewCM.DanhMuc.NhomDichVus.Dto;
using NewCM.DbEntities;
using NewCM.Global;

namespace NewCM.DanhMuc.NhomDichVus
{
    [AbpAuthorize(PermissionNames.Pages_DanhMucNhomDichVu), DisableAuditing]
    public class DanhMucNhomDichVuAppService : AsyncCrudAppService<DanhMucNhomDichVu, DanhMucNhomDichVuDto, int, GetAllDanhMucNhomDichVuInput, CreateDanhMucNhomDichVuDto, DanhMucNhomDichVuDto>, IDanhMucNhomDichVuAppService
    {
        public DanhMucNhomDichVuAppService(IRepository<DanhMucNhomDichVu> danhMucNhomDichVuRepository

        ) : base(danhMucNhomDichVuRepository)
        {
            
        }

        [HttpPost]
        public override Task<DanhMucNhomDichVuDto> Update(DanhMucNhomDichVuDto input)
        {
            return base.Update(input);
        }

        protected override IQueryable<DanhMucNhomDichVu> CreateFilteredQuery(GetAllDanhMucNhomDichVuInput input)
        {
            string Keyword = GlobalFunction.RegexFormat(input.Keyword);

            return base.CreateFilteredQuery(input)
                .WhereIf(!string.IsNullOrWhiteSpace(Keyword), w => w.Name.Contains(Keyword));
        }

        protected override IQueryable<DanhMucNhomDichVu> ApplySorting(IQueryable<DanhMucNhomDichVu> query, GetAllDanhMucNhomDichVuInput input)
        {
            return base.ApplySorting(query, input)
                .OrderBy(o => o.Name);
        }
    }
}
