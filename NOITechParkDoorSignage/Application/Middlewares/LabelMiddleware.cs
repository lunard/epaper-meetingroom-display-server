using NOITechParkDoorSignage.Application.Services.Interfaces;

namespace NOITechParkDoorSignage.Application.ActionFilters
{
    public class LabelMiddleware
    {
        private readonly ILabelService _labelService;
        private readonly RequestDelegate _next;

        public LabelMiddleware(
            RequestDelegate next,
            ILabelService labelService)
        {
            _labelService = labelService;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;

            var labelID = request.Headers["label-id"];
            if (string.IsNullOrEmpty(labelID))
                throw new ArgumentNullException("The header value 'label-id' has not been found in the request");

            var roomEmail = _labelService.GetRoomEmailByLabelID(labelID);
            if (string.IsNullOrEmpty(roomEmail))
                throw new ArgumentNullException($"The label ID ({labelID}) is not associated to a Room (see the appsettings.json configuration file)");

            context.Items["roomEmail"] = roomEmail;

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
