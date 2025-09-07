using Seletivo.Application.Shared.Contracts;
using Seletivo.Application.Shared.Dtos;
using Serilog;

namespace Infrastructure.Shared.Services
{
    public class LoggerService : ILoggerService
    {
        public void LogInfo(CreateLogRequest request)
        {
            Log.ForContext("Action", request.Action)
                        .ForContext("Request", request.Request)
                        .ForContext("Response", request.Response)
                        .ForContext("UserId", request.UserId)
                        .Information(request.Message);
        }

        public void LogWarning(CreateLogRequest request)
        {
            Log.ForContext("Action", request.Action)
                        .ForContext("Request", request.Request)
                        .ForContext("Response", request.Response)
                        .ForContext("UserId", null)
                        .Warning(request.Message);
        }
    }
}