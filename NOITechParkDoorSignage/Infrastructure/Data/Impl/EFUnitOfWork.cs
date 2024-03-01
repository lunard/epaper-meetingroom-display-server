using NOITechParkDoorSignage.Infrastructure.Data.Interfaces;

namespace NOITechParkDoorSignage.Infrastructure.Data.Impl
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly EFRoomContext _context;
        public EFUnitOfWork(EFRoomContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
