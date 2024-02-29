using System.ComponentModel.DataAnnotations;

namespace NOITechParkDoorSignage.Domain.Room
{
    public class Room
    {
        public Room()
        {
            Events = new List<Event>();
            AirQualityMeasures = new List<AirQualityMeasure>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Email { get; set; }
        public string DisplayName { get; set; }

        public string Location { get; set; }

        public List<Event> Events { get; set; }

        public List<AirQualityMeasure> AirQualityMeasures { get; set; }
    }
}
