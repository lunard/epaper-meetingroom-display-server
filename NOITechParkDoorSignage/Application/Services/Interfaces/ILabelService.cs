// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace NOITechParkDoorSignage.Application.Services.Interfaces
{
    public interface ILabelService
    {
        string GetRoomEmailByLabelID(string labelID);
    }
}
