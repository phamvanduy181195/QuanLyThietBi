using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Abp.Authorization.Users;
using Abp.Domain.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Abp.UI;
using NewCM.Authorization.Roles;
using NewCM.MultiTenancy;

namespace NewCM.Authorization.Users
{
    public class UserRegistrationManager : DomainService
    {
        public IAbpSession AbpSession { get; set; }

        private readonly TenantManager _tenantManager;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        //private readonly IPasswordHasher<User> _passwordHasher;

        public UserRegistrationManager(
            TenantManager tenantManager,
            UserManager userManager,
            RoleManager roleManager
            //IPasswordHasher<User> passwordHasher
            )
        {
            _tenantManager = tenantManager;
            _userManager = userManager;
            _roleManager = roleManager;
            //_passwordHasher = passwordHasher;

            AbpSession = NullAbpSession.Instance;
            LocalizationSourceName = NewCMConsts.LocalizationSourceName;
        }

        public async Task<User> RegisterAsync(
            string phoneNumber,
            string emailAddress,
            string emailConfirmationCode,
            string plainPassword,
            bool isActive,
            bool isCustomer)
        {
            CheckForTenant();

            var tenant = await GetActiveTenantAsync();

            var user = new User
            {
                TenantId = tenant.Id,
                Name = "",
                Surname = "",
                EmailAddress = emailAddress ?? "",
                EmailConfirmationCode = emailConfirmationCode,
                IsActive = isActive,
                UserName = phoneNumber,
                PhoneNumber = phoneNumber,
                IsCustomer = isCustomer,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();
           
            //foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
            //{
            //    user.Roles.Add(new UserRole(tenant.Id, user.Id, defaultRole.Id));
            //}

            await _userManager.InitializeOptionsAsync(tenant.Id);

            try
            {
                CheckErrors(await _userManager.CreateAsync(user, plainPassword));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Email") && ex.Message.Contains("is already taken"))
                {
                    throw new UserFriendlyException(L("Identity.DuplicateEmail", user.EmailAddress));
                }
                else if (ex.Message.Contains("User") && ex.Message.Contains("is already taken"))
                {
                    throw new UserFriendlyException(L("Identity.DuplicateUserName", user.UserName));
                }
                else
                {
                    throw new UserFriendlyException("Unknow Error: " + ex.Message);
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return user;
        }

        private void CheckForTenant()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                throw new InvalidOperationException("Can not register host users!");
            }
        }

        private async Task<Tenant> GetActiveTenantAsync()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return await GetActiveTenantAsync(AbpSession.TenantId.Value);
        }

        private async Task<Tenant> GetActiveTenantAsync(int tenantId)
        {
            var tenant = await _tenantManager.FindByIdAsync(tenantId);
            if (tenant == null)
            {
                throw new UserFriendlyException(L("UnknownTenantId{0}", tenantId));
            }

            if (!tenant.IsActive)
            {
                throw new UserFriendlyException(L("TenantIdIsNotActive{0}", tenantId));
            }

            return tenant;
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
