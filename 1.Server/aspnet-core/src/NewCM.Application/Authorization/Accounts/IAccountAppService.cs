using System.Threading.Tasks;
using Abp.Application.Services;
using NewCM.Authorization.Accounts.Dto;

namespace NewCM.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);

        
        Task<string> UpdateInfo(UpdateInfoDto input);
    }
}
