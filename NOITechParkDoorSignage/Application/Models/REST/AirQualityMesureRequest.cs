// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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