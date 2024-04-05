// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using NOITechParkDoorSignage.Domain.Room;

namespace NOITechParkDoorSignage.Application.Services.Interfaces
{
    public interface ICalendarSourceService
    {
        // Sync the local database with the source of truth
        Task SyncWithSource();

        Task<bool> AddEventByRoomEmail(Event evt, string roomEmail);

        Task<bool> DeleteAllEventsAddedByTheLabel(string roomEmail);
    }
}
