using System.Threading.Tasks;
using Abp.Application.Services;
using HizliSu.Sessions.Dto;

namespace HizliSu.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
