// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using NOITechParkDoorSignage.Application.Models.REST;
using NOITechParkDoorSignage.Domain.Room;

namespace NOITechParkDoorSignage.Application.Services.Interfaces
{
    public interface IRoomService
    {
        Task<RoomStatusViewModel> GetRoomStatus(string roomEmail);

        Task<bool> AddBooking(string roomEmail, int eventDuration);

        Task<bool> RemoveLabelBookings(string roomEmail);

        Task<RoomViewModel> GetRoomInfo(string roomEmail);
        Task<AirQualityMeasure> GetAirQuality(string roomEmail);
        Task<bool> AddAirQualityMeasure(string roomEmail, AirQualityMeasure airQuality);
    }
}
