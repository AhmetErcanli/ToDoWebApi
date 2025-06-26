using System.Net;
using System.Text.Json;

namespace ToDoWebApi.Middlewares
{
    /// <summary>
    /// Uygulama genelinde yakalanmayan hataları ele alır.
    /// Hataları loglar ve istemciye JSON formatında standart bir mesaj döner.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// HTTP isteği işlerken oluşan beklenmedik hataları yakalar.
        /// </summary>
        /// <param name="context">HTTP istek bağlamı</param>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // bir sonraki middleware'i çalıştır
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Global Error"); // hatayı logla

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var error = new
                {
                    message = "An unexpected error occurred.",
                    detail = ex.Message // Not: production'da bu kısmı göstermek riskli olabilir
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(error, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
