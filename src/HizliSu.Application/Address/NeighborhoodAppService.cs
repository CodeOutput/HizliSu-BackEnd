using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using HizliSu.Address.Dto;
using HizliSu.Address.Request;

namespace HizliSu.Address
{
    [AbpAuthorize()]
    public class NeighborhoodAppService : ApplicationService, INeighborhoodAppService
    {
        private readonly IRepository<Neighborhood, long> _neighborhoodRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public NeighborhoodAppService(IUnitOfWorkManager unitOfWorkManager, IRepository<Neighborhood, long> neighborhoodRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _neighborhoodRepository = neighborhoodRepository;
        }
  
        public async Task<ListResultDto<NeighborhoodDto>> GetNeighborhoodListAsync(long districtId, bool? isActive)
        {


            List<Neighborhood> neighborhoodList;
            if (isActive.HasValue)
            {
                neighborhoodList = await _neighborhoodRepository.GetAllListAsync(x => x.DistrictId == districtId && x.IsActive == isActive.Value);
            }
            else
            {
                neighborhoodList = await _neighborhoodRepository.GetAllListAsync(x => x.DistrictId == districtId);
            }
            return new ListResultDto<NeighborhoodDto>(ObjectMapper.Map<List<NeighborhoodDto>>(neighborhoodList));
        }
           

        public async Task<NeighborhoodDto> SaveNeighborhoodAsync(NeighborhoodRequest request)
        {
 
            if (request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new UserFriendlyException("Mahalle ismi giriniz!");
            }

            var neighborhood = await _neighborhoodRepository.FirstOrDefaultAsync(x => x.Name.ToLower() == request.Name.ToLower());
            if (neighborhood == null)
            {
                neighborhood = new Neighborhood()
                {
                    Name = request.Name,
                    DistrictId = request.DistrictId,
                    IsActive = request.IsActive
                };
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {

                    var neighborhoodId = await _neighborhoodRepository.InsertAndGetIdAsync(neighborhood);
                    neighborhood.Id = neighborhoodId;
                    await unitOfWork.CompleteAsync();
                }

                return ObjectMapper.Map<NeighborhoodDto>(neighborhood); 
            }

            throw new UserFriendlyException("Mahalle zaten girilmiştir!");
        }

        public async Task<NeighborhoodDto> UpdateNeighborhoodAsync(long neighborhoodId, NeighborhoodRequest request)
        {
            if (request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new UserFriendlyException("Mahalle ismi giriniz!");
            }

            var neighborhood = await _neighborhoodRepository.FirstOrDefaultAsync(neighborhoodId);
            if (neighborhood == null)
            {

                throw new UserFriendlyException("Kayıt bulunamadı!"); 
            }

            neighborhood.Name = request.Name;
            neighborhood.IsActive = request.IsActive;
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                await _neighborhoodRepository.UpdateAsync(neighborhood);
                await unitOfWork.CompleteAsync();
            }

            return ObjectMapper.Map<NeighborhoodDto>(neighborhood);
        }

        public async Task<NeighborhoodDto> DeleteNeighborhoodAsync(long neighborhoodId)
        {
            var neighborhood = await _neighborhoodRepository.FirstOrDefaultAsync(neighborhoodId);
            if (neighborhood == null)
            {
                throw new UserFriendlyException("Kayıt bulunamadı!");
            }
            await _neighborhoodRepository.DeleteAsync(neighborhoodId);
            return ObjectMapper.Map<NeighborhoodDto>(neighborhood);
        }
    }
}
