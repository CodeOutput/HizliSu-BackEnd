using Abp.Application.Services;
using HizliSu.MultiTenancy.Dto;

namespace HizliSu.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

