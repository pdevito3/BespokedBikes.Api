namespace Infrastructure.Persistence.Seeders
{

    using AutoBogus;
    using Domain.Entities;
    using Infrastructure.Persistence.Contexts;
    using System.Linq;

    public static class CustomerSeeder
    {
        public static void SeedSampleCustomerData(BespokedBikesDbContext context)
        {
            if (!context.Customers.Any())
            {
                context.Customers.Add(new AutoFaker<Customer>());
                context.Customers.Add(new AutoFaker<Customer>());
                context.Customers.Add(new AutoFaker<Customer>());

                context.SaveChanges();
            }
        }
    }
}