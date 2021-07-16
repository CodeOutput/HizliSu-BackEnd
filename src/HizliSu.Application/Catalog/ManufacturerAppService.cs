using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using HizliSu.Catalog.Dto;
using HizliSu.Catalog.Request;
using HizliSu.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace HizliSu.Catalog
{
    [AbpAuthorize()]
    public class ManufacturerAppService : ApplicationService, IManufacturerAppService
    {
        private readonly IRepository<Manufacturer, long> _manufacturerRepository;
        private readonly IRepository<Facility, long> _facilityRepository;
        private readonly IRepository<FacilityAttribute, long> _facilityAttributeRepository;
        private readonly IRepository<File, long> _fileRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ManufacturerAppService(IUnitOfWorkManager unitOfWorkManager, IRepository<File, long> fileRepository, IRepository<Manufacturer, long> manufacturerRepository, IRepository<Facility, long> facilityRepository, IRepository<FacilityAttribute, long> facilityAttributeRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _fileRepository = fileRepository;
            _manufacturerRepository = manufacturerRepository;
            _facilityRepository = facilityRepository;
            _facilityAttributeRepository = facilityAttributeRepository;
        }

        public async Task<ListResultDto<ManufacturerDto>> GetManufacturerListAsync()
        {
            List<Manufacturer> manufacturerList = (await _manufacturerRepository.GetAllListAsync()).OrderBy(x=>x.SortOrder).ToList();
            return new ListResultDto<ManufacturerDto>(ObjectMapper.Map<List<ManufacturerDto>>(manufacturerList));
        }

        public async Task<ManufacturerDto> GetManufacturerDetailAsync(long id)
        {
            var manufacturer = await _manufacturerRepository.GetAllIncluding(i=>i.Image).FirstOrDefaultAsync(x=>x.Id == id);
            if (manufacturer == null)
            {
                throw new UserFriendlyException("Üretici bulunamadı!");
            }
            return ObjectMapper.Map<ManufacturerDto>(manufacturer);
        }

        public async Task<ManufacturerDto> SaveManufacturerAsync([FromForm] ManufacturerRequest request)
        {
            if(request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");

            if (request.Content == null)
            {
                throw new UserFriendlyException("Dosya yüklemediniz!");
            }

            long length = request.Content?.Length??0;

            await using var fileStream = request.Content.OpenReadStream();
            byte[] bytes = new byte[length];
            await fileStream.ReadAsync(bytes, 0, (int) request.Content.Length);


            var file = new File()
            {
                FileName = request.Content.FileName,
                UserFileName = request.Content.FileName,
                Length = request.Content.Length,
                ContentType = request.Content.ContentType.ToLower(new CultureInfo("en-US", false)),
                Content = bytes
            };

            var manufacturer = new Manufacturer()
            {
                Name = request.Name,
                Description = request.Description,
                SortOrder = request.SortOrder
            };
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var addImageId = await _fileRepository.InsertAndGetIdAsync(file);
                manufacturer.ImageId = addImageId;
                var manufacturerId = await _manufacturerRepository.InsertAndGetIdAsync(manufacturer);
                manufacturer.Id = manufacturerId;
                await unitOfWork.CompleteAsync();
            }
            return ObjectMapper.Map<ManufacturerDto>(manufacturer);
        }





        public async Task<ManufacturerDto> UpdateManufacturerAsync(long manufacturerId, [FromForm] ManufacturerRequest request)
        {
            if (request == null)
                throw new UserFriendlyException("Kaydedilecek veri bulunamadı!");
  
            long length = request.Content?.Length??0;

            var manufacturer = await _manufacturerRepository.FirstOrDefaultAsync(manufacturerId);

            if (manufacturer  == null)
                throw new UserFriendlyException("Üretici bulunamadı!");
            

            var file = await _fileRepository.FirstOrDefaultAsync(manufacturer.ImageId);


            if (length > 0)
            {
                if (request.Content != null)
                {
                    await using var fileStream = request.Content.OpenReadStream();
                    byte[] bytes = new byte[length];
                    await fileStream.ReadAsync(bytes, 0, (int) request.Content.Length);
                    
                    file.FileName = request.Content.FileName;
                    file.UserFileName = request.Content.FileName;
                    file.Length = request.Content.Length;
                    file.ContentType = request.Content.ContentType.ToLower(new CultureInfo("en-US", false));
                    file.Content = bytes;
                }
            }

            manufacturer.Name = request.Name;
            manufacturer.Description = request.Description;
            manufacturer.SortOrder = request.SortOrder;
            
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                if (length > 0)
                {
                   await _fileRepository.UpdateAsync(file);
                }
                await _manufacturerRepository.UpdateAsync(manufacturer);
                await unitOfWork.CompleteAsync();
            }
            return ObjectMapper.Map<ManufacturerDto>(manufacturer);
        }

        public async Task<ManufacturerDto> DeleteManufacturerAsync(long manufacturerId)
        {
            var manufacturer = await _manufacturerRepository.FirstOrDefaultAsync(manufacturerId);
            if (manufacturer == null)
            {
                throw new UserFriendlyException("Üretici bulunamadı!");
            }

            await _manufacturerRepository.DeleteAsync(manufacturerId);
            await _fileRepository.DeleteAsync(manufacturer.ImageId);

            return ObjectMapper.Map<ManufacturerDto>(manufacturer);
        }

        public async Task<ListResultDto<FacilityDto>> GetFacilityListAsync(long manufacturerId)
        {
            List<Facility> facilityList = await (_facilityRepository.GetAllIncluding(i=>i.FacilityAttributes).Where(x=>x.ManufacturerId == manufacturerId)).ToListAsync();
            var list = new ListResultDto<FacilityDto>(ObjectMapper.Map<List<FacilityDto>>(facilityList));
            return list;
        }

        public async Task<FacilityDto> SaveFacilityAsync(FacilityRequest request)
        {
            var manufacturer = await _manufacturerRepository.FirstOrDefaultAsync(request.ManufacturerId);
            if (manufacturer == null)
            {
                throw new UserFriendlyException("Üretici bulunamadı!");
            }

            var entity = new Facility()
            {
                Name = request.Name,
                Address = request.Address,
                Description = request.Description,
                ManufacturerId = request.ManufacturerId,
            };

            Facility ins = await _facilityRepository.InsertAsync(entity);
            return ObjectMapper.Map<FacilityDto>(ins);
        }

        public async Task<FacilityDto> UpdateFacilityAsync(long facilityId, FacilityRequest request)
        {
            Facility entity = await _facilityRepository.FirstOrDefaultAsync(facilityId);
            if (entity == null)
            {
                throw new UserFriendlyException("Tesis bulunamadı!");
            }
            
            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Address = request.Address;


            Facility upd = await _facilityRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FacilityDto>(upd);
        }

        public async Task<FacilityDto> DeleteFacilityAsync(long facilityId)
        {
            var entity = await _facilityRepository.FirstOrDefaultAsync(facilityId);
            if (entity == null)
            {
                throw new UserFriendlyException("Tesis bulunamadı!");
            }

            await _facilityRepository.DeleteAsync(facilityId);

            return ObjectMapper.Map<FacilityDto>(entity);
        }

        public async Task<FacilityAttributeDto> SaveFacilityAttributeAsync(FacilityAttributeRequest request)
        {
            var facility = await _facilityRepository.FirstOrDefaultAsync(request.FacilityId);
            if (facility == null)
            {
                throw new UserFriendlyException("Tesis bulunamadı!");
            }

            var entity = new FacilityAttribute()
            {
                Key = request.Key,
                Value = request.Value,
                SortOrder = request.SortOrder,
                FacilityId = request.FacilityId,
            };

            var ins = await _facilityAttributeRepository.InsertAsync(entity);
            return ObjectMapper.Map<FacilityAttributeDto>(ins);
        }

        public async Task<FacilityAttributeDto> UpdateFacilityAttributeAsync(long facilityAttributeId, FacilityAttributeRequest request)
        {
            var entity = await _facilityAttributeRepository.FirstOrDefaultAsync(facilityAttributeId);
            if (entity == null)
            {
                throw new UserFriendlyException("Tesise ait özellik bulunamadı!");
            }

            entity.Key = request.Key;
            entity.Value = request.Value;
            entity.SortOrder = request.SortOrder;
            
            var upd = await _facilityAttributeRepository.UpdateAsync(entity);
            return ObjectMapper.Map<FacilityAttributeDto>(upd);
        }

        public async Task<FacilityAttributeDto> DeleteFacilityAttributeAsync(long facilityAttributeId)
        {
            var entity = await _facilityAttributeRepository.FirstOrDefaultAsync(facilityAttributeId);
            if (entity == null)
            {
                throw new UserFriendlyException("Tesise ait özellik bulunamadı!");
            }

            await _facilityAttributeRepository.DeleteAsync(facilityAttributeId);

            return ObjectMapper.Map<FacilityAttributeDto>(entity);
        }
    }
}
