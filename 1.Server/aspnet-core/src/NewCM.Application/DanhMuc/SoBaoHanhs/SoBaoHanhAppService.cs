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
using NewCM.DanhMuc.SoBaoHanhs.Dto;
using NewCM.DbEntities;
using NewCM.Global;

namespace NewCM.DanhMuc.SoBaoHanhs
{
    [AbpAuthorize, DisableAuditing]
    public class SoBaoHanhAppService : AsyncCrudAppService<SoBaoHanh, SoBaoHanhDto, int, GetAllSoBaoHanhInput, CreateSoBaoHanhDto, SoBaoHanhDto>, ISoBaoHanhAppService
    {
        private IRepository<SoBaoHanh> _soBaoHanhRepository;
        private readonly IGlobalCache _globalCache;

        public SoBaoHanhAppService(IRepository<SoBaoHanh> soBaoHanhRepository,
            IGlobalCache globalCache
        ) : base(soBaoHanhRepository)
        {
            LocalizationSourceName = NewCMConsts.LocalizationSourceName;

            _soBaoHanhRepository = soBaoHanhRepository;
            _globalCache = globalCache;
        }

        public async Task<List<SoBaoHanhDto>> GetAllNew()
        {
            await CheckIsCustomer();

            var query = await _soBaoHanhRepository.GetAll()
                            .Where(w => w.CreatorUserId == (long)AbpSession.UserId)
                            .OrderByDescending(o => o.QuanTam)
                            .ToListAsync();

            List<SoBaoHanhDto> result = new List<SoBaoHanhDto>();
            foreach (var item in query)
            {
                var newItemDto = ObjectMapper.Map<SoBaoHanhDto>(item);

                if (item.Image != null)
                    newItemDto.ImageBase64 = Convert.ToBase64String(item.Image);

                if (newItemDto.NgayMua.HasValue)
                    newItemDto.SoNgayConBaoHanh = (newItemDto.NgayMua.Value.AddMonths(newItemDto.SoThangBaoHanh) - newItemDto.NgayMua.Value).Days;

                result.Add(newItemDto);
            }

            return result;
        }

        public override async Task<SoBaoHanhDto> Get(EntityDto<int> input)
        {
            await CheckIsCustomer();

            return ObjectMapper.Map<SoBaoHanhDto>(await _soBaoHanhRepository.FirstOrDefaultAsync(w => w.Id == input.Id && w.CreatorUserId == (long)AbpSession.UserId));
        }

        public override async Task<SoBaoHanhDto> Create(CreateSoBaoHanhDto input)
        {
            await CheckIsCustomer();

            if (!string.IsNullOrWhiteSpace(input.ImageBase64))
                input.Image = Convert.FromBase64String(input.ImageBase64);

            return await base.Create(input);
        }

        [HttpPost]
        public override async Task<SoBaoHanhDto> Update(SoBaoHanhDto input)
        {
            await CheckIsCustomer();

            var updateItem = await _soBaoHanhRepository.FirstOrDefaultAsync(w => w.Id == input.Id && w.CreatorUserId == (long)AbpSession.UserId);

            if (updateItem != null)
            {
                ObjectMapper.Map(input, updateItem);

                if (!string.IsNullOrWhiteSpace(input.ImageBase64))
                    updateItem.Image = Convert.FromBase64String(input.ImageBase64);
            }
            else
                throw new UserFriendlyException(L("SoBaoHanhIsNotFound"));

            return input;
        }

        public override async Task Delete(EntityDto<int> input)
        {
            await CheckIsCustomer();

            await _soBaoHanhRepository.DeleteAsync(w => w.Id == input.Id && w.CreatorUserId == (long)AbpSession.UserId);
        }

        [HttpPost]
        public async Task QuanTam(QuanTamInput input)
        {
            await CheckIsCustomer();

            var updateItem = await _soBaoHanhRepository.FirstOrDefaultAsync(w => w.Id == input.Id && w.CreatorUserId == (long)AbpSession.UserId);

            if (updateItem != null)
            {
                updateItem.QuanTam = input.QuanTam;
            }
            else
                throw new UserFriendlyException(L("SoBaoHanhIsNotFound"));
        }

        protected override IQueryable<SoBaoHanh> CreateFilteredQuery(GetAllSoBaoHanhInput input)
        {
            string Keyword = GlobalFunction.RegexFormat(input.Keyword);

            return base.CreateFilteredQuery(input)
                .Where(w => w.CreatorUserId == (long)AbpSession.UserId)
                .WhereIf(!string.IsNullOrWhiteSpace(Keyword), w => w.Name.Contains(Keyword));
        }

        protected override IQueryable<SoBaoHanh> ApplySorting(IQueryable<SoBaoHanh> query, GetAllSoBaoHanhInput input)
        {
            return base.ApplySorting(query, input)
                .OrderBy(o => o.Name);
        }

        private async Task CheckIsCustomer()
        {
            var UserCache = await _globalCache.GetUserCache((long)AbpSession.UserId);

            if (UserCache.Id == 0)
                throw new UserFriendlyException(L("UserIsNotLogin"));

            if (!UserCache.IsCustomer || UserCache.CustomerId == 0)
                throw new UserFriendlyException(L("UserIsNotCustomer"));
        }
    }
}
