using Entities.DTOs.Service;
using Entities.Model;
using System.Linq.Expressions;

namespace Services.AppServices.ServiceServices
{
    public interface IServiceService 
    {
        Task<ServiceResult<List<ServiceDto>>> GetAll();
        Task<ServiceResult<List<ServiceDto>>> Where(Expression<Func<Service, bool>> expression);
        Task<ServiceResult<ServiceDto?>> GetByIdAsync(Guid id);
        Task<ServiceResult<CreateServiceResponse>> CreateAsync(CreateServiceRequest request);
        Task<ServiceResult<bool>> SoftDeleteAsync(Guid id);
        Task<ServiceResult<bool>> DeleteAsync(Guid id);
        Task<ServiceResult<bool>> UpdateAsync(UpdateServiceRequest request);
      
    }
}