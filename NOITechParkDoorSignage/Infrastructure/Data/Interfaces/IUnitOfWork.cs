namespace NOITechParkDoorSignage.Infrastructure.Data.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
