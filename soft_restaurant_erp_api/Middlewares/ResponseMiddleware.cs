using Newtonsoft.Json;

namespace soft_restaurant_erp_api.Middlewares
{
    public class ResponseMiddleware: IMiddleware
    {
        private string generateResponse(string? res, string? error)
        {
            res = string.IsNullOrEmpty(res) ? "null" : res;
            error = string.IsNullOrEmpty(error) ? "null" : error;

            return $"{{ \"data\": {res}, \"error\": {error}}}";
        }
        private async Task Error(HttpContext context, int status, string msg)
        {
            string exMess = JsonConvert.SerializeObject(msg);
            string newResp = generateResponse(null, exMess);
            await ServerError(context, status, newResp);
        }

        private async Task ServerError(HttpContext context, int status, string msg)
        {
            context.Response.StatusCode = status;
            await context.Response.WriteAsync(msg);
        }

        private async Task<string> ReadResponse(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
                if (context.Response.StatusCode >= 200 && context.Response.StatusCode <= 299)
                {
                    string contentType = context.Response.ContentType?.ToLowerInvariant() ?? "";
                    bool isJson = contentType.StartsWith("application/json");
                    bool isTextPlain = contentType.StartsWith("text/plain");
                    if (context.Response.ContentType == null || isJson || isTextPlain)
                    {
                        string resp = await ReadResponse(context.Response);
                        if (isTextPlain)
                        {
                            resp = JsonConvert.SerializeObject(resp);
                        }
                        string newResp = generateResponse(resp, null);
                        await context.Response.WriteAsync(newResp);
                    }
                }
            }
            catch (Exception ex)
            {
                await Error(context, 500, ex.Message);
            }
        }
    }
}
