using NOITechParkDoorSignage.Domain.Room;

namespace NOITechParkDoorSignage.Infrastructure.Data.Interfaces
{
    public interface IRoomRepository
    {
        IUnitOfWork UnitOfWork { get; }

        Task<List<Room>> List();

        Task<Room> Get(string email);
    }
}
