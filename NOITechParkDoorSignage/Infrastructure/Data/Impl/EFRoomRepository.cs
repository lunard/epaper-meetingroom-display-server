// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.EntityFrameworkCore;
using NOITechParkDoorSignage.Domain.Room;
using NOITechParkDoorSignage.Infrastructure.Data.Interfaces;

namespace NOITechParkDoorSignage.Infrastructure.Data.Impl
{
    public class EFRoomRepository : IRoomRepository
    {
        private readonly EFRoomContext _context;
        private readonly IUnitOfWork _unitOfWork;

        IUnitOfWork IRoomRepository.UnitOfWork => _unitOfWork;

        public EFRoomRepository(
            EFRoomContext context,
            IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Room>> List()
        {
            return await _context.Rooms.Include(r => r.Events).ToListAsync();
        }
        public async Task<Room?> Get(string email)
        {
            return await _context.Rooms
            .Include(r => r.Events)
            .Include(r => r.AirQualityMeasures)
            .FirstOrDefaultAsync(r => r.Email == email);
        }
    }
}
