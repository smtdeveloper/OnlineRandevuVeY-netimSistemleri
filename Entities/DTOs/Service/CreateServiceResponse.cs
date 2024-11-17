namespace Entities.DTOs.Service;

public class CreateServiceResponse
{
    public Guid Id { get; set; }

    public CreateServiceResponse(Guid id)
    {
        Id = id;
    }

    // Parametresiz constructor (opsiyonel)
    public CreateServiceResponse()
    {
    }
}