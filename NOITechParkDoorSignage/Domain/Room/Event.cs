using System.ComponentModel.DataAnnotations;

namespace NOITechParkDoorSignage.Domain.Room
{
    public class Event
    {
        [Key]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public List<string> Attendees { get; set; }
        public string Organizer { get; set; }
        public Guid RoomId { get; set; }
        public Room Room { get; set; }
    }
}
