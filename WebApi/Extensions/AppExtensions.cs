namespace WebApi.Extensions
{
    using Microsoft.AspNetCore.Builder;

    public static class AppExtensions
    {
        #region Swagger Region - Do Not Delete            
        public static void UseSwaggerExtension(this IApplicationBuilder app)
        {
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
        #endregion
    }
}
