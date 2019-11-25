using Abp.Authorization;
using NewCM.Authorization.Roles;
using NewCM.Authorization.Users;

namespace NewCM.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
