using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using NewCM.Authorization;
using NewCM.DbEntities;
using NewCM.Global;
using NewCM.HeThong.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NewCM.HeThong
{
    [AbpAuthorize(PermissionNames.Pages_QuanLyKhuyenMai)]
    public class QuanLyKhuyenMaiAppService : AsyncCrudAppService<TinTuc, TinTucDto, int, GetAllTinTucInput, CreateTinTucDto, TinTucDto>, IQuanLyKhuyenMaiAppService
    {
        public QuanLyKhuyenMaiAppService(IRepository<TinTuc> tinTucRepository): base(tinTucRepository)
        {
            LocalizationSourceName = NewCMConsts.LocalizationSourceName;
        }

        public override async Task<PagedResultDto<TinTucDto>> GetAll(GetAllTinTucInput input)
        {
            var TinTucs = await base.GetAll(input);
            foreach (var TinTuc in TinTucs.Items)
            {
                TinTuc.ImageBase64 = Convert.ToBase64String(TinTuc.Image);
                TinTuc.Image = null;
            }

            return TinTucs;
        }

        public override async Task<TinTucDto> Get(EntityDto<int> input)
        {
            var TinTuc = await base.Get(input);
            TinTuc.ImageBase64 = Convert.ToBase64String(TinTuc.Image);
            TinTuc.Image = null;

            return TinTuc;
        }

        public override Task<TinTucDto> Create(CreateTinTucDto input)
        {
            if (!string.IsNullOrWhiteSpace(input.ImageBase64))
                input.Image = Convert.FromBase64String(input.ImageBase64);

            return base.Create(input);
        }

        [HttpPost]
        public override Task<TinTucDto> Update(TinTucDto input)
        {
            if (!string.IsNullOrWhiteSpace(input.ImageBase64))
                input.Image = Convert.FromBase64String(input.ImageBase64);

            return base.Update(input);
        }

        protected override IQueryable<TinTuc> CreateFilteredQuery(GetAllTinTucInput input)
        {
            string Keyword = GlobalFunction.RegexFormat(input.Keyword);

            return base.CreateFilteredQuery(input)
                .Where(w => w.Type == (int)GlobalConst.LoaiTinTuc.KhuyenMai)
                .WhereIf(!string.IsNullOrWhiteSpace(Keyword), w => w.Name.Contains(Keyword));
        }
    }
}
