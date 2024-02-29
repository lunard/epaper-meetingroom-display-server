using NOITechParkDoorSignage.Application.Models;
using NOITechParkDoorSignage.Application.Services.Interfaces;

namespace NOITechParkDoorSignage.Application.Services.Impl
{
    public class ConfigurationLabelServices: ILabelService
    {
        private readonly ConfigurationRoomsOption _roomConfigurationOption = new ConfigurationRoomsOption();
        public ConfigurationLabelServices(IConfiguration configuration)
        {
            _roomConfigurationOption.Rooms = configuration.GetSection("Rooms").Get<List<RoomOption>>();
        }

        public string GetRoomEmailByLabelID(string labelID)
        {
            return _roomConfigurationOption.Rooms.FirstOrDefault(x => x.AssociatedLabelMACs.Contains(labelID))?.Email;
        }
    }
}
