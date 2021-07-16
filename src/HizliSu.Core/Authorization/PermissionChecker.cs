using Abp.Authorization;
using HizliSu.Authorization.Roles;
using HizliSu.Authorization.Users;

namespace HizliSu.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
