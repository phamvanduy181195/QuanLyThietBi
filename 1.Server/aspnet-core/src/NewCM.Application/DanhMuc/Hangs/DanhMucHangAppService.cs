using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using NewCM.Authorization;
using NewCM.DanhMuc.Hangs.Dto;
using NewCM.DbEntities;
using NewCM.Global;

namespace NewCM.DanhMuc.Hangs
{
    [AbpAuthorize(PermissionNames.Pages_DanhMucHang), DisableAuditing]
    public class DanhMucHangAppService : AsyncCrudAppService<DanhMucHang, DanhMucHangDto, int, GetAllDanhMucHangInput, CreateDanhMucHangDto, DanhMucHangDto>, IDanhMucHangAppService
    {
        public DanhMucHangAppService(IRepository<DanhMucHang> danhMucHangRepository

        ) : base(danhMucHangRepository)
        {
            
        }

        [HttpPost]
        public override Task<DanhMucHangDto> Update(DanhMucHangDto input)
        {
            return base.Update(input);
        }

        protected override IQueryable<DanhMucHang> CreateFilteredQuery(GetAllDanhMucHangInput input)
        {
            string Keyword = GlobalFunction.RegexFormat(input.Keyword);

            return base.CreateFilteredQuery(input)
                .WhereIf(!string.IsNullOrWhiteSpace(Keyword), w => w.Name.Contains(Keyword));
        }

        protected override IQueryable<DanhMucHang> ApplySorting(IQueryable<DanhMucHang> query, GetAllDanhMucHangInput input)
        {
            return base.ApplySorting(query, input)
                .OrderBy(o => o.Name);
        }
    }
}
