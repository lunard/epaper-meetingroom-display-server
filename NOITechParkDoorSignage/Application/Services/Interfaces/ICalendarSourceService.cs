using NOITechParkDoorSignage.Domain.Room;

namespace NOITechParkDoorSignage.Application.Services.Interfaces
{
    public interface ICalendarSourceService
    {
        // Sync the local database with the source of truth
        Task SyncWithSource();

        Task<bool> AddEventByRoomEmail(Event evt, string roomEmail);

        Task<bool> DeleteAllEventsAddedByTheLabel(string roomEmail);
    }
}
