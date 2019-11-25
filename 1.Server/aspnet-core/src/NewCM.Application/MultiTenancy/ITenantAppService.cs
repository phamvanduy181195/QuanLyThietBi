using Abp.Application.Services;
using Abp.Application.Services.Dto;
using NewCM.MultiTenancy.Dto;

namespace NewCM.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

