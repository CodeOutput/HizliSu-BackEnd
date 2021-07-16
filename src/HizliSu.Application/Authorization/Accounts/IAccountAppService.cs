using System.Threading.Tasks;
using Abp.Application.Services;
using HizliSu.Authorization.Accounts.Dto;

namespace HizliSu.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
