using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.SharedLibrary.MiddleWare
{
    public class GlobalException(RequestDelegate next)
    {
        public async async InvokeAsync(HttpContext context)
        {
            // declare variables
            string message = "Sorry, Internal server error occured, please try again.";
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
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            //dislay message to cient
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
