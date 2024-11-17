using System.Net;

namespace Services;

public class ServiceResult<T>
{
    public T? Data { get; set; }
    public List<string>? ErrorMessage { get; set; }
    public bool IsSuccess => ErrorMessage == null || ErrorMessage.Count == 0;
    public bool IsFail => !IsSuccess;
    public HttpStatusCode Status { get; set; }

    public ServiceResult<T> Success(T data, HttpStatusCode status = HttpStatusCode.OK) 
    {
        return new ServiceResult<T> { Data = data, Status = status };
    }

    public ServiceResult<T> Fail(List<string> errorMessages, HttpStatusCode status = HttpStatusCode.BadRequest)
    {
        return new ServiceResult<T> { ErrorMessage = errorMessages  , Status = status };
    }

    public ServiceResult<T> Fail(string errorMessage, HttpStatusCode status = HttpStatusCode.BadRequest)
    {
        return new ServiceResult<T> { ErrorMessage = [errorMessage] , Status = status  };
    }
}