using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using HizliSu.Address.Dto;
using HizliSu.Address.Request;

namespace HizliSu.Address
{
    public interface IUserAddressAppService : IApplicationService
    {
        Task<ListResultDto<UserAddressDto>> GetAuthUserAddressListAsync();
        Task<UserAddressDto> GetAuthUserAddressDetailAsync(long addressId);
        Task<UserAddressDto> SaveUserAddressAsync(UserAddressRequest request);
        Task<UserAddressDto> UpdateUserAddressAsync(long userAddressId, UserAddressRequest request);
        Task<UserAddressDto> DeleteUserAddressAsync(long userAddressId);
    }
}
