using Guardian.Application.Shared.Contracts;
using Guardian.Application.Wrappers;
using System.Text.Json;

namespace Guardian.WebApi.Middlewares
{
    public class InternalErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerService _loggerService;

        public InternalErrorMiddleware(RequestDelegate next, ILoggerService loggerService)
        {
            _next = next;
            _loggerService = loggerService;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);

                HttpResponse response = context.Response;
                response.ContentType = "application/json";

                Response<string> responseModel = Response<string>.Failure(
                    errors: ["Erro interno identificado. Entre em contato com um administrador e tente novamente."]
                );

                response.StatusCode = StatusCodes.Status500InternalServerError;

                _loggerService.LogWarning(new()
                {
                    Action = context.GetEndpoint().DisplayName,
                    Message = error.Message
                });

                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}