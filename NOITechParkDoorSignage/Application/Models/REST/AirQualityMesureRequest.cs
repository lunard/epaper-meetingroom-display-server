namespace NOITechParkDoorSignage.Application.Models.REST
{
    /// <summary>
    /// Simplified Calendar view model for the ESP32 e-Paper label 
    /// </summary>
    public class AirQualityMesureRequest
    {
        public decimal CO2 { get; set; }
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
    }
}