namespace Entities.DTOs.Service;

public class UpdateServiceRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}