namespace WebApi.Extensions
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using NSwag;
    using System;

    public static class ServiceExtensions
    {
        #region Swagger Region - Do Not Delete
            public static void AddSwaggerExtension(this IServiceCollection services)
            {
                services.AddSwaggerDocument(config => {
                    config.PostProcess = document =>
                    {
                        document.Info.Version = "v1";
                        document.Info.Title = "BespokedBikes";
                        document.Info.Description = "Our API uses a REST based design, leverages the JSON data format, and relies upon HTTPS for transport. We respond with meaningful HTTP response codes and if an error occurs, we include error details in the response body. API Documentation is at bespokedbikes.com/dev/docs";
                        document.Info.Contact = new OpenApiContact
                        {
                            Name = "BespokedBikes",
                            Email = "devsupport@bespokedbikes.com",
                            Url = "https://www.bespokedbikes.com",
                        };
                        document.Info.License = new OpenApiLicense()
                        {
                            Name = $"Copyright {DateTime.Now.Year}",
                        };
                    };
                });
            }
        #endregion

        public static void AddApiVersioningExtension(this IServiceCollection services)
        {
            services.AddApiVersioning(config =>
            {
                // Default API Version
                config.DefaultApiVersion = new ApiVersion(1, 0);
                // use default version when version is not specified
                config.AssumeDefaultVersionWhenUnspecified = true;
                // Advertise the API versions supported for the particular endpoint
                config.ReportApiVersions = true;
            });
        }

        public static void AddCorsService(this IServiceCollection services, string policyName)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName,
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }
    }
}
