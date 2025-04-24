using CleanArch.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace CleanArch.API.Extensions;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration.GetValue<string>(Constant.JwtKey) ?? throw new InvalidOperationException();
        var issuer = configuration.GetValue<string>(Constant.Issuer) ?? throw new InvalidOperationException();
        var audience = configuration.GetValue<string>(Constant.Audience) ?? throw new InvalidOperationException();

        var key = Encoding.ASCII.GetBytes(jwtKey);

        var tokenValidationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(jwt => {
            jwt.SaveToken = true;
            jwt.TokenValidationParameters = tokenValidationParams;
            jwt.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        IsSuccess = false,
                        Message = "Token inválido o no presente",
                        Data = context.ErrorDescription
                    };

                    var json = JsonConvert.SerializeObject(response);
                    return context.Response.WriteAsync(json);
                }
            };
        });

        services.AddAuthorization();

        return services;
    }
}
