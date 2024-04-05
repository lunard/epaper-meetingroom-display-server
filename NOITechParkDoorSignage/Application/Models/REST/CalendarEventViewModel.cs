// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace NOITechParkDoorSignage.Application.Models.REST
{
    /// <summary>
    /// Simplified Calendar view model for the ESP32 e-Paper label 
    /// </summary>
    public class CalendarEventViewModel
    {
        public string Title { get; set; }
        public string Organizer { get; set; }
        public string StartAt { get; set; }
        public string EndAt { get; set; }
        public bool BookedByLabel { get; set; }
    }
}
