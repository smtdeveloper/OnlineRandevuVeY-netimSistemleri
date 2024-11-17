namespace Entities.DTOs.Auth;

public class CreateUserResponse
{
    public Guid Id { get; set; }

    public CreateUserResponse(Guid id)
    {
        Id = id;
    }
}