using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using HizliSu.Address.Dto;
using HizliSu.Address.Request;
using Microsoft.EntityFrameworkCore;

namespace HizliSu.Address
{
    [AbpAuthorize()]
    public class UserAddressAppService : ApplicationService, IUserAddressAppService
    {
        private readonly IRepository<UserAddress, long> _userAddressRepository;
        private readonly IRepository<Orders.Order, long> _orderRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserAddressAppService(IUnitOfWorkManager unitOfWorkManager, IRepository<UserAddress, long> userAddressRepository, IRepository<Orders.Order, long> orderRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _userAddressRepository = userAddressRepository;
            _orderRepository = orderRepository;
        }
  
        public async Task<ListResultDto<UserAddressDto>> GetAuthUserAddressListAsync()
        {

            List<UserAddress> userAddressList = await _userAddressRepository.GetAllIncluding(i=>i.City,ii=>ii.District,b=>b.Neighborhood).Where(x=>x.UserId == AbpSession.UserId && x.IsActive).OrderBy(x=>x.Title).ToListAsync();
            return new ListResultDto<UserAddressDto>(ObjectMapper.Map<List<UserAddressDto>>(userAddressList));
        }
        public async Task<UserAddressDto> GetAuthUserAddressDetailAsync(long addressId)
        {

            UserAddress userAddress = await _userAddressRepository.GetAllIncluding(i => i.City, ii => ii.District, b => b.Neighborhood)
                .FirstOrDefaultAsync(x => x.UserId == AbpSession.UserId && x.Id == addressId);
            return ObjectMapper.Map<UserAddressDto>(userAddress);
        }

        public async Task<UserAddressDto> SaveUserAddressAsync(UserAddressRequest request)
        {
 
            if (request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");

            if (string.IsNullOrEmpty(request.Title))
            {
                throw new UserFriendlyException("Adres başlığı giriniz!");
            }
            
            if (string.IsNullOrEmpty(request.DoorNumber))
            {
                throw new UserFriendlyException("Kapı numarası giriniz!");
            }
            
            if (string.IsNullOrEmpty(request.No))
            {
                throw new UserFriendlyException("Bina numarası giriniz!");
            }
            
            if (string.IsNullOrEmpty(request.AddressDescription))
            {
                throw new UserFriendlyException("Adres tarifi giriniz!");
            }
            
            if (string.IsNullOrEmpty(request.StreetName))
            {
                throw new UserFriendlyException("Cadde/Sokak bilgisi giriniz!");
            }

            if (!AbpSession.UserId.HasValue || AbpSession.UserId.Value<1)
            {
                throw new UserFriendlyException("Kullanıcı bulunamadı!");
            }

            if (request.CityId < 1)
            {
                throw new UserFriendlyException("Şehir seçiniz!");
            }

            
            if (request.DistrictId < 1)
            {
                throw new UserFriendlyException("İlçe seçiniz!");
            }

            
            if (request.NeighborhoodId < 1)
            {
                throw new UserFriendlyException("Mahalle seçiniz!");
            }



            var userAddress = await _userAddressRepository.FirstOrDefaultAsync(x => x.Title.ToLower() == request.Title.ToLower());
            if (userAddress == null)
            {
                userAddress = new UserAddress()
                {
                    UserId = AbpSession.UserId.Value,
                    CityId = request.CityId,
                    DistrictId = request.DistrictId,
                    NeighborhoodId = request.NeighborhoodId,
                    AddressDescription = request.AddressDescription,
                    DoorNumber = request.DoorNumber,
                    No = request.No,
                    StreetName = request.StreetName,
                    Title = request.Title,
                    PhoneNumber = request.PhoneNumber,
                    IsActive = true
                };
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {

                    var userAddressId = await _userAddressRepository.InsertAndGetIdAsync(userAddress);
                    userAddress.Id = userAddressId;
                    await unitOfWork.CompleteAsync();
                }

                return ObjectMapper.Map<UserAddressDto>(userAddress); 
            }

            throw new UserFriendlyException("Adres başlığı zaten girilmiştir!");
        }

        public async Task<UserAddressDto> UpdateUserAddressAsync(long userAddressId, UserAddressRequest request)
        {
            if (request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");

            if (string.IsNullOrEmpty(request.Title))
            {
                throw new UserFriendlyException("Adres başlığı giriniz!");
            }

            if (string.IsNullOrEmpty(request.DoorNumber))
            {
                throw new UserFriendlyException("Kapı numarası giriniz!");
            }

            if (string.IsNullOrEmpty(request.No))
            {
                throw new UserFriendlyException("Bina numarası giriniz!");
            }

            if (string.IsNullOrEmpty(request.AddressDescription))
            {
                throw new UserFriendlyException("Adres tarifi giriniz!");
            }

            if (string.IsNullOrEmpty(request.StreetName))
            {
                throw new UserFriendlyException("Cadde/Sokak bilgisi giriniz!");
            }

            if (!AbpSession.UserId.HasValue || AbpSession.UserId.Value < 1)
            {
                throw new UserFriendlyException("Kullanıcı bulunamadı!");
            }

            if (request.CityId < 1)
            {
                throw new UserFriendlyException("Şehir seçiniz!");
            }


            if (request.DistrictId < 1)
            {
                throw new UserFriendlyException("İlçe seçiniz!");
            }


            if (request.NeighborhoodId < 1)
            {
                throw new UserFriendlyException("Mahalle seçiniz!");
            }

            var userTitleHas = await _userAddressRepository.FirstOrDefaultAsync(x =>x.Id != userAddressId && x.Title.ToLower() == request.Title.ToLower());
            if (userTitleHas != null)
            {
                throw new UserFriendlyException("Adres başlığı zaten daha önceden eklenmiştir.");
            }

            var userAddress = await _userAddressRepository.FirstOrDefaultAsync(userAddressId);
            if (userAddress == null)
            {
                throw new UserFriendlyException("Kayıt bulunamadı!");
            }

            //userAddress.UserId = AbpSession.UserId.Value;
            userAddress.CityId = request.CityId;
            userAddress.DistrictId = request.DistrictId;
            userAddress.NeighborhoodId = request.NeighborhoodId;
            userAddress.AddressDescription = request.AddressDescription;
            userAddress.DoorNumber = request.DoorNumber;
            userAddress.No = request.No;
            userAddress.StreetName = request.StreetName;
            userAddress.Title = request.Title;
            userAddress.PhoneNumber = request.PhoneNumber;
     
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {

                await _userAddressRepository.UpdateAsync(userAddress);
                await unitOfWork.CompleteAsync();
            }

            return ObjectMapper.Map<UserAddressDto>(userAddress);

        }

        public async Task<UserAddressDto> DeleteUserAddressAsync(long userAddressId)
        {
            var userAddress = await _userAddressRepository.FirstOrDefaultAsync(userAddressId);
            if (userAddress == null)
            {
                throw new UserFriendlyException("Kayıt bulunamadı!");
            }

            if (!AbpSession.UserId.HasValue || AbpSession.UserId.Value < 1)
            {
                throw new UserFriendlyException("Kullanıcı bulunamadı!");
            }

            var orderAdressList = await _orderRepository.GetAllListAsync(x => x.UserAddressId == userAddressId);
            if (orderAdressList.Any())
            {
                throw new UserFriendlyException("Siparişte kayıtlı olan adresi silemezsiniz!");
            }
            var userAddressList = await _userAddressRepository.GetAllListAsync(x =>
                x.Id != userAddressId && x.UserId == AbpSession.UserId.Value && x.IsActive);
            if (userAddressList.Any())
            {
                await _userAddressRepository.DeleteAsync(userAddressId);
                return ObjectMapper.Map<UserAddressDto>(userAddress);
            }

            throw new UserFriendlyException("En az bir adet adres kaydı sistemde bulunmalıdır!");
        }
    }
}
