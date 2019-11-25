using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Auditing;
using Microsoft.AspNetCore.Hosting;
using NewCM.Configuration;
using NewCM.Sessions.Dto;

namespace NewCM.Sessions
{
    [DisableAuditing]
    public class SessionAppService : NewCMAppServiceBase, ISessionAppService
    {
        private readonly string AndroidVersion;
        private readonly string IosVersion;

        public SessionAppService(IHostingEnvironment env)
        {
            var _appConfiguration = env.GetAppConfiguration();
            AndroidVersion = _appConfiguration["App:VersionInfo:AndroidVersion"] ?? "1.0";
            IosVersion = _appConfiguration["App:VersionInfo:IosVersion"] ?? "1.0";
        }

        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    AndroidVersion = AndroidVersion,
                    IosVersion = IosVersion,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>()
                }
            };

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
            }

            if (AbpSession.UserId.HasValue)
            {
                var CurrentUser = await GetCurrentUserAsync();
                output.User = ObjectMapper.Map<UserLoginInfoDto>(CurrentUser);
                output.User.ProfilePictureBase64 = CurrentUser.ProfilePicture != null ? Convert.ToBase64String(CurrentUser.ProfilePicture) : AppConsts.ProfilePicture;
            }

            return output;
        }
    }
}
