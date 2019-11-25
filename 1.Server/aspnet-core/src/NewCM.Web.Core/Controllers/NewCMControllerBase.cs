using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace NewCM.Controllers
{
    public abstract class NewCMControllerBase: AbpController
    {
        protected NewCMControllerBase()
        {
            LocalizationSourceName = NewCMConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
