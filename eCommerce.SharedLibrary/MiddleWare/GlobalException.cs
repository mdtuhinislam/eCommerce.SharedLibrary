using System.Net;
using System.Text.Json;
using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.SharedLibrary.MiddleWare
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // declare variables
            string message = "Sorry, Internal server error occurred, please try again.";
            string title = "Error";
            int statusCode = (int)HttpStatusCode.InternalServerError;

            try
            {
                await next(context);

                //checking if too many request exception
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "Too many requests!";
                    statusCode = StatusCodes.Status429TooManyRequests;

                    await ModifyHeader(context, title, message, statusCode);
                }

                //Unauthorized
                if(context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    message = "You are not authorized to access!";
                    statusCode = StatusCodes.Status401Unauthorized;

                    await ModifyHeader(context, title, message, statusCode);
                }

                //If request if forbidden // status code 403
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of service";
                    message = "You are not allowed/required to access!";
                    statusCode = StatusCodes.Status403Forbidden;

                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {

                //Log original exception
                LogException.LogExceptions(ex);

                // time out 408
                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Time out";
                    message = "Request timeout!";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }

                //If none of the exception do default
                await ModifyHeader(context, title, message, statusCode);
            }
        }

        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            //display message to client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Status = statusCode,
                Detail = message,
                Title = title
            }), CancellationToken.None);

            return;

        }
    }
}
