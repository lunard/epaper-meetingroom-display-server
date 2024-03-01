namespace NOITechParkDoorSignage.Application.Models
{
    public class ConfigurationRoomsOption
    {
        public ConfigurationRoomsOption()
        {
            Rooms = new List<RoomOption>();
        }
        public List<RoomOption> Rooms { get; set; }
    }

    public class RoomOption
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Email { get; set; }
        public List<string> AssociatedLabelMACs { get; set; }
    }
}
