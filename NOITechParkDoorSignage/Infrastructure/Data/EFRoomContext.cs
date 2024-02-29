using Microsoft.EntityFrameworkCore;
using NOITechParkDoorSignage.Domain.Room;

namespace NOITechParkDoorSignage.Infrastructure.Data
{
    public class EFRoomContext : DbContext
    {
        public EFRoomContext(DbContextOptions<EFRoomContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "CalendarDB");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Events)
                .WithOne(e => e.Room)
                .HasForeignKey(e => e.RoomId);
        }
        public DbSet<Room> Rooms { get; set; }
    }
}
