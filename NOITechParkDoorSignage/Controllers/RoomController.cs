// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.AspNetCore.Mvc;
using NOITechParkDoorSignage.Application.ActionFilters;
using NOITechParkDoorSignage.Application.Models.REST;
using NOITechParkDoorSignage.Application.Services.Interfaces;
using NOITechParkDoorSignage.Domain.Room;

namespace NOITechParkDoorSignage.Controllers
{
    [Route("api/room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILabelService _labelService;
        private readonly ILogger<RoomController> _logger;
        public RoomController(
            IRoomService roomService,
            ILabelService labelService,
            ILogger<RoomController> logger)
        {
            _roomService = roomService ?? throw new ArgumentNullException(typeof(ICalendarSourceService).Name);
            _labelService = labelService ?? throw new ArgumentNullException(typeof(ILabelService).Name);
            _logger = logger ?? throw new ArgumentNullException(typeof(ILogger<RoomController>).Name);
        }

        // Get all events of a room
        [HttpGet("status")]
        public async Task<RoomStatusViewModel> GetEvents()
        {
            RoomStatusViewModel status = null;
            try
            {
                status = await _roomService.GetRoomStatus(HttpContext.Items["roomEmail"] as string);
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                // write back the error message
                await Response.WriteAsync(ex.Message);

                _logger.LogError(ex, "Error getting room status");

                return null;
            }

            return status;
        }

        // Get all events of a room
        [HttpGet("airquality")]
        public async Task<AirQualityMeasure> GetAirQuality()
        {
            AirQualityMeasure status = null;
            try
            {
                status = await _roomService.GetAirQuality(HttpContext.Items["roomEmail"] as string);
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                // write back the error message
                await Response.WriteAsync(ex.Message);

                _logger.LogError(ex, "Error getting room air quality");

                return null;
            }

            return status;
        }

        [HttpPost("airquality")]
        public async Task<bool> AddAirQualityMeasure(AirQualityMesureRequest request)
        {
            bool status = false;
            try
            {
                status = await _roomService.AddAirQualityMeasure(HttpContext.Items["roomEmail"] as string, new AirQualityMeasure()
                {
                    CO2 = request.CO2,
                    Humidity = request.Humidity,
                    Temperature = request.Temperature,
                    MeasureDate = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                // write back the error message
                await Response.WriteAsync(ex.Message);

                _logger.LogError(ex, "Error adding room air quality measure");

                return false;
            }

            return status;
        }

        [HttpGet("book/{bookDuration}")]
        public async Task<IActionResult> BookRoom(int bookDuration)
        {
            try
            {
                if (!await _roomService.AddBooking(HttpContext.Items["roomEmail"] as string, bookDuration))
                    return BadRequest();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error booking room");
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpDelete("book")]
        public async Task<IActionResult> DeleteAllEvents()
        {
            try
            {
                if (!await _roomService.RemoveLabelBookings(HttpContext.Items["roomEmail"] as string))
                    return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all events");
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpGet("")]
        public async Task<RoomViewModel> GetRoomInfo()
        {
            try
            {
                return await _roomService.GetRoomInfo(HttpContext.Items["roomEmail"] as string);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting room info");
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return null;
            }
        }
    }
}
