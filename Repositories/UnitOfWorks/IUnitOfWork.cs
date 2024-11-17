namespace Repositories.UnitOfWorks;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
