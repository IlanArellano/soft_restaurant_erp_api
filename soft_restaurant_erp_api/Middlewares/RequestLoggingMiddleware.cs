using System.Text;

namespace soft_restaurant_erp_api.Middlewares
{

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();
            var requestBody = await ReadRequestBody(context.Request);

            LogRequest(context, requestBody);

            context.Request.Body.Position = 0;

            await _next(context);
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.Body.Position = 0; // Asegurarse de empezar desde el principio
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                return body;
            }
        }

        private void LogRequest(HttpContext context, string requestBody)
        {
            _logger.LogInformation($"Request {context.Request.Method} {context.Request.Path}: {requestBody}");
        }
    }
}
