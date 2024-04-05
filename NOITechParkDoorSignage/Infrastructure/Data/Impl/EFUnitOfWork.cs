// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using NOITechParkDoorSignage.Infrastructure.Data.Interfaces;

namespace NOITechParkDoorSignage.Infrastructure.Data.Impl
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly EFRoomContext _context;
        public EFUnitOfWork(EFRoomContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
