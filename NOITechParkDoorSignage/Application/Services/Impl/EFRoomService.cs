using Newtonsoft.Json;
using NOITechParkDoorSignage.Application.ExtensionMethods;
using NOITechParkDoorSignage.Application.Models.REST;
using NOITechParkDoorSignage.Application.Services.Interfaces;
using NOITechParkDoorSignage.Domain.Room;
using NOITechParkDoorSignage.Infrastructure.Data.Interfaces;

namespace NOITechParkDoorSignage.Application.Services.Impl
{
    public class EFRoomService : IRoomService
    {
        private readonly ILogger<EFRoomService> _logger;
        private readonly IRoomRepository _roomRepository;
        private readonly ICalendarSourceService _calendarSourceService;
        public EFRoomService(
            IRoomRepository roomRepository,
            ICalendarSourceService calendarSourceService,
            ILogger<EFRoomService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(typeof(ILogger<EFRoomService>).Name);
            _roomRepository = roomRepository ?? throw new ArgumentNullException(typeof(IRoomRepository).Name);
            _calendarSourceService = calendarSourceService ?? throw new ArgumentNullException(typeof(ICalendarSourceService).Name);
        }

        public async Task<RoomStatusViewModel> GetRoomStatus(string roomEmail)
        {
            var room = await GetRoom(roomEmail);

            var result = new RoomStatusViewModel();

            var currentEvent = room
                .Events
                .OrderBy(e => e.StartDate)
                .FirstOrDefault(e => e.StartDate <= DateTime.Now.ToUniversalTime() && e.EndDate > DateTime.Now.ToUniversalTime());

            var nextEvent = room
                .Events
                .OrderBy(e => e.StartDate)
                .FirstOrDefault(e => e.StartDate >= DateTime.Now.ToUniversalTime());

            var minutesToTheEndOfTheDay = (int)(DateTime.Now.AddDays(1).Date - DateTime.Now).TotalMinutes;

            result.IsFree = currentEvent == null;
            result.TimeToNextEvent = (nextEvent == null) ? minutesToTheEndOfTheDay : nextEvent.StartDate.TotalMinutesFromNow();
            result.CurrentEvent = currentEvent.GetEventViewModel();
            result.NextEvent = nextEvent.GetEventViewModel();

            _logger.LogInformation($"Room {roomEmail} status: {JsonConvert.SerializeObject(result)}");
            return result;
        }

        public async Task<bool> AddBooking(string roomEmail, int eventDuration)
        {
            var status = await GetRoomStatus(roomEmail);
            if (status == null || !status.IsFree)
            {
                var msg = $"Room {roomEmail} is booked for the next {status.TimeToNextEvent} minutes";
                _logger.LogError(msg);
                return false;
            }

            var room = await GetRoom(roomEmail);

            var newEvent = new Event
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMinutes(eventDuration),
                RoomId = room.Id,
                Title = "In place booking",
                Description = "In place booking",
                Organizer = $"{room.Email}",
                Attendees = new List<string> { $"{room.Email}" },
                Location = $"{room.Location}",
            };

            if (!await _calendarSourceService.AddEventByRoomEmail(newEvent, roomEmail))
            {
                var msg = $"Failed to add event to the calendar for room {roomEmail}";
                _logger.LogError(msg);
                return false;
            }

            return true;
        }

        public async Task<bool> RemoveLabelBookings(string roomEmail)
        {
            var status = await GetRoomStatus(roomEmail);
            if (status == null || status.IsFree)
            {
                var msg = $"Room {roomEmail} is already free";
                _logger.LogWarning(msg);
                return true;
            }

            var room = await GetRoom(roomEmail);

            if (!await _calendarSourceService.DeleteAllEventsAddedByTheLabel(roomEmail))
            {
                var msg = $"Failed to remove event from the calendar for room {roomEmail}";
                _logger.LogError(msg);
                throw new Exception(msg);
            }

            return true;
        }

        public async Task<RoomViewModel> GetRoomInfo(string roomEmail)
        {
            var room = await GetRoom(roomEmail);

            return new RoomViewModel
            {
                Email = room.Email,
                DisplayName = room.DisplayName,
                Location = room.Location
            };
        }

        public async Task<AirQualityMeasure> GetAirQuality(string roomEmail)
        {
            var room = await GetRoom(roomEmail);
            var measure = room.AirQualityMeasures.OrderByDescending(a => a.MeasureDate).FirstOrDefault();
            if (measure == null)
            {
                _logger.LogWarning($"No air quality measure found for room {roomEmail}");
                return new AirQualityMeasure
                {
                    MeasureDate = DateTime.Now,
                    CO2 = 0,
                    Humidity = 0,
                    Temperature = 0
                };
            }
            return measure;
        }

        public async Task<bool> AddAirQualityMeasure(string roomEmail, AirQualityMeasure airQuality)
        {
            try
            {
                var room = await GetRoom(roomEmail);
                room.AirQualityMeasures.Add(airQuality);
                await _roomRepository.UnitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Air quality measure added for room {roomEmail}: {JsonConvert.SerializeObject(airQuality)}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting air quality");
                return false;
            }
        }

        private async Task<Room> GetRoom(string roomEmail)
        {
            var room = await _roomRepository.Get(roomEmail);
            if (room == null)
            {
                var msg = $"Room with email {roomEmail} not found";
                _logger.LogError(msg);
                throw new Exception(msg);
            }
            return room;
        }
    }
}
