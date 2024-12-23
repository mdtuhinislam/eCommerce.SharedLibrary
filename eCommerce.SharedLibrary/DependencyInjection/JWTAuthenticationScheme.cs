﻿using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace eCommerce.SharedLibrary.DependencyInjection
{
    public static class JWTAuthenticationScheme
    {
        public static IServiceCollection AddJWTAuthenticationScheme(this IServiceCollection services, IConfiguration config)
        {
            try
            {
                // add jwt service
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer("Bearer", options =>
                    {
                        var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
                        string issuer = config.GetSection("Authentication:Issuer").Value!;
                        string audience = config.GetSection("Authentication:Audience").Value!;

                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = false,
                            ValidateIssuerSigningKey = true,
                            ValidAudience = audience,
                            ValidIssuer = issuer,
                            IssuerSigningKey = new SymmetricSecurityKey(key)
                        };

                    });
                return services;
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }
}
