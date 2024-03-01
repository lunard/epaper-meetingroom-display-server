using Hangfire;
using NOITechParkDoorSignage.Application.Services.Interfaces;

namespace NOITechParkDoorSignage.Application.BackgroundJobs
{
    public class MicrosoftGraphJob
    {
        private readonly ILogger<MicrosoftGraphJob> _logger;
        private readonly ICalendarSourceService _roomService;

        public MicrosoftGraphJob(
            ILogger<MicrosoftGraphJob> logger,
            ICalendarSourceService roomService
            )
        {
            _logger = logger;
            _roomService = roomService ?? throw new ArgumentNullException(typeof(ICalendarSourceService).Name);
        }

        [JobDisplayName("Sync Office 365 data")]
        [AutomaticRetry(Attempts = 0)]
        [DisableConcurrentExecution(timeoutInSeconds: 300)]
        public async Task SyncOffice365Data()
        {
            await _roomService.SyncWithSource();
        }
    }
}
