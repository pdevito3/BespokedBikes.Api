namespace Infrastructure.Persistence.Seeders
{

    using AutoBogus;
    using Bogus;
    using Domain.Entities;
    using Infrastructure.Persistence.Contexts;
    using System.Linq;

    public static class SaleSeeder
    {
        public static void SeedSampleSaleData(BespokedBikesDbContext context)
        {
            if (!context.Sales.Any())
            {
                var salespeople = context.Salespersons.ToList();
                var products = context.Products.ToList();
                var customers = context.Customers.ToList();

                for (var eachCustomer = 1; eachCustomer <= 20; eachCustomer++)
                {
                    var productIndex = new Faker().Random.Number(0, products.Count()-1);
                    var productKey = products[productIndex].ProductId;

                    var customerIndex = new Faker().Random.Number(0, customers.Count()-1);
                    var customerKey = customers[customerIndex].CustomerId;

                    var salespersonIndex = new Faker().Random.Number(0, salespeople.Count()-1);
                    var salespersonKey = salespeople[salespersonIndex].SalespersonId;

                    context.Sales.Add(new Faker<Sale>()
                        .RuleFor(fake => fake.ProductId, fake => productKey)
                        .RuleFor(fake => fake.SalespersonId, fake => salespersonKey)
                        .RuleFor(fake => fake.CustomerId, fake => customerKey)
                        .RuleFor(fake => fake.SaleDate, fake => fake.Date.Recent())
                        );
                }

                context.SaveChanges();

                context.Products.RemoveRange(context.Products.Where(p => p.Name == null));
                context.Salespersons.RemoveRange(context.Salespersons.Where(p => p.FirstName == null));
                context.Customers.RemoveRange(context.Customers.Where(p => p.FirstName == null));
                context.SaveChanges();
            }
        }
    }
}