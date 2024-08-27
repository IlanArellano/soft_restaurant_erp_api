namespace soft_restaurant_erp_api.Middlewares
{
    public class RewindMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Stream originalBody = context.Response.Body;
            try
            {
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    await next(context);

                    memStream.Position = 0;
                    string responseBody = new StreamReader(memStream).ReadToEnd();

                    memStream.Position = 0;
                    context.Response.ContentLength = memStream.Length;
                    await memStream.CopyToAsync(originalBody);
                }
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
    }
}
