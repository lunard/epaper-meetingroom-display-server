namespace NOITechParkDoorSignage.Application.Models.REST
{
    public class RoomStatusViewModel
    {
        public bool IsFree { get; set; }
        public int TimeToNextEvent { get; set; }
        public CalendarEventViewModel ? CurrentEvent { get; set; }
        public CalendarEventViewModel ? NextEvent { get; set; }
    }
}
