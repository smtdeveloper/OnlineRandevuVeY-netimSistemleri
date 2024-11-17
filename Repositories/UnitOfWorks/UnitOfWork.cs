namespace Repositories.UnitOfWorks;
public class UnitOfWork(AppointmentDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
}