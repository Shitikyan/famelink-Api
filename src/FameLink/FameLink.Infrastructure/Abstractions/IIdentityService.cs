using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace FameLink.Infrastructure.Abstractions
{
    public interface IIdentityService
    {
        string GetUserId();

        string GetUserName();

        string GetUserEmail();

        IIdentity GetCurrentUser();

        IEnumerable<string> GetUserRoles();
    }
}
