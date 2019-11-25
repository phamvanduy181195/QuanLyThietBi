using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using NewCM.Configuration.Dto;

namespace NewCM.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : NewCMAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
