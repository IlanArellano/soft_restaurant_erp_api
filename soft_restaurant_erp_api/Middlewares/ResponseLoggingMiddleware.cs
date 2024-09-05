namespace soft_restaurant_erp_api.Middlewares
{

    public class ResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseLoggingMiddleware> _logger;

        public ResponseLoggingMiddleware(RequestDelegate next, ILogger<ResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                LogResponse(context, text);

                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private void LogResponse(HttpContext context, string responseBody)
        {
            _logger.LogInformation($"Response: {context.Response.StatusCode}, Body: {responseBody}");
        }
    }
}
