using AutoMapper;
using Entities.DTOs.Service;
using Entities.Model;
using Microsoft.EntityFrameworkCore;
using Repositories.RepositoriesDal.ServiceDal;
using Repositories.UnitOfWorks;
using System.Linq.Expressions;

namespace Services.AppServices.ServiceServices
{
    internal class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServiceService(IServiceRepository serviceRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CreateServiceResponse>> CreateAsync(CreateServiceRequest request)
        {
            var service = _mapper.Map<Service>(request);
            service.CreatedDate = DateTime.UtcNow;

            await _serviceRepository.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();

            var response = new CreateServiceResponse(service.Id);

            return new ServiceResult<CreateServiceResponse>().Success(response);
        }


        public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            
            if (service == null) 
                return new ServiceResult<bool>().NotFound("Servis bulunamadı.");

            _serviceRepository.Delete(service);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult<bool>().Success(true);
        }

        public async Task<ServiceResult<List<ServiceDto>>> GetAll()
        {
            var services = await _serviceRepository.GetAll().ToListAsync();
            var serviceDtos = _mapper.Map<List<ServiceDto>>(services);
            return new ServiceResult<List<ServiceDto>>().Success(serviceDtos);
        }

        public async Task<ServiceResult<ServiceDto?>> GetByIdAsync(Guid id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            

            var serviceDto = _mapper.Map<ServiceDto>(service);
            return new ServiceResult<ServiceDto?>().Success(serviceDto);
        }

        public async Task<ServiceResult<bool>> SoftDeleteAsync(Guid id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
                return new ServiceResult<bool>().NotFound("Servis bulunamadı.");

            service.IsDelete = true;
            service.DeletedDate = DateTime.UtcNow;
            _serviceRepository.Update(service);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult<bool>().Success(true);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(UpdateServiceRequest request)
        {
            var service = await _serviceRepository.GetByIdAsync(request.Id);
            if (service == null)
                return new ServiceResult<bool>().NotFound("Servis bulunamadı.");

            service.Name = request.Name;          
            _serviceRepository.Update(service);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult<bool>().Success(true);
        }

        public async Task<ServiceResult<List<ServiceDto>>> Where(Expression<Func<Service, bool>> expression)
        {
            var services = await _serviceRepository.Where(expression).ToListAsync();
            var serviceDtos = _mapper.Map<List<ServiceDto>>(services);
            return new ServiceResult<List<ServiceDto>>().Success(serviceDtos);
        }
    }
}
