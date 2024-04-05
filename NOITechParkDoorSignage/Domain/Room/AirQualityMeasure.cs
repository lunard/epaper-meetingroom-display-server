// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.ComponentModel.DataAnnotations;

namespace NOITechParkDoorSignage.Domain.Room
{
    public class AirQualityMeasure
    {
        [Key]
        public DateTime MeasureDate { get; set; }
        public decimal CO2 { get; set; }
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
    }
}