using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.MiddleWare
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Extract info. from request header

            var signedHeader = context.Request.Headers["Api-Gateway"];

            //NULL means the request is not coming from API Gateway the 503 service unavailable
            if(signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry!, the requested service is unavailable!");
                return;

            }
            else
            {
                await next(context);
            }
        }
    }
}
