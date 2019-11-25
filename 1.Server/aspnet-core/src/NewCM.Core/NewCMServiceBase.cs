using Abp;

namespace NewCM
{
    public abstract class NewCMServiceBase: AbpServiceBase
    {
        protected NewCMServiceBase()
        {
            LocalizationSourceName = NewCMConsts.LocalizationSourceName;
        }
    }
}
