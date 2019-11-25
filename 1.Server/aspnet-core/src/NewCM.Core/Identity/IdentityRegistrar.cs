using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NewCM.Authorization;
using NewCM.Authorization.Roles;
using NewCM.Authorization.Users;
using NewCM.Editions;
using NewCM.Localization;
using NewCM.MultiTenancy;

namespace NewCM.Identity
{
    public static class IdentityRegistrar
    {
        public static IdentityBuilder Register(IServiceCollection services)
        {
            services.AddLogging();

            return services.AddAbpIdentity<Tenant, User, Role>()
                .AddAbpTenantManager<TenantManager>()
                .AddAbpUserManager<UserManager>()
                .AddAbpRoleManager<RoleManager>()
                .AddAbpEditionManager<EditionManager>()
                .AddAbpUserStore<UserStore>()
                .AddAbpRoleStore<RoleStore>()
                .AddAbpLogInManager<LogInManager>()
                .AddAbpSignInManager<SignInManager>()
                .AddAbpSecurityStampValidator<SecurityStampValidator>()
                .AddAbpUserClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
                .AddPermissionChecker<PermissionChecker>()

                // localize identity error messages
                //.AddErrorDescriber<LocalizedIdentityErrorDescriber>()
                
                .AddDefaultTokenProviders();
        }
    }
}
