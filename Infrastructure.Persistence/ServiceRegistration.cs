namespace Infrastructure.Persistence
{
    using Infrastructure.Persistence.Contexts;
    using Application.Interfaces.Discount;
    using Application.Interfaces.Sale;
    using Application.Interfaces.Customer;
    using Application.Interfaces.Salesperson;
    using Application.Interfaces.Product;
    using Infrastructure.Persistence.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Sieve.Services;
    using System;

    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region DbContext -- Do Not Delete          
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<BespokedBikesDbContext>(options =>
                    options.UseInMemoryDatabase($"BespokedBikes"));
            }
            else
            {
                services.AddDbContext<BespokedBikesDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("BespokedBikes"),
                        builder => builder.MigrationsAssembly(typeof(BespokedBikesDbContext).Assembly.FullName)));
            }
            #endregion

            services.AddScoped<SieveProcessor>();

            #region Repositories -- Do Not Delete
            services.AddScoped<IDiscountRepository, DiscountRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ISalespersonRepository, SalespersonRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            #endregion
        }
    }
}
