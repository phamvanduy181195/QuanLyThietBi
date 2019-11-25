using System.Threading.Tasks;
using Abp.Application.Services;
using NewCM.Sessions.Dto;

namespace NewCM.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
