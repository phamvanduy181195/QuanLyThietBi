using System.Threading.Tasks;
using NewCM.Configuration.Dto;

namespace NewCM.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
