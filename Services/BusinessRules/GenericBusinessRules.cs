using Services;
using System.Net;

namespace Services.BusinessRules;

public static class GenericBusinessRules
{
    public static ServiceResult<bool>? CheckEntityNotNull<T>(T entity, string entityName) where T : class
    {
        if (entity == null)
        {
            var errorMessage = $"{entityName} not found.";
            return new ServiceResult<bool>().Fail(errorMessage, HttpStatusCode.NotFound);
        }

        return null;
    }
}
