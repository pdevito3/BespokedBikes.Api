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

                for (var eachCustomer = 1; eachCustomer <= 10; eachCustomer++)
                {
                    var productIndex = new Faker().Random.Number(1, products.Count());
                    var salespersonIndex = new Faker().Random.Number(1, salespeople.Count());
                    var productKey = products[productIndex].ProductId;
                    var salespersonKey = salespeople[salespersonIndex].SalespersonId;

                    context.Sales.Add(new Faker<Sale>()
                        .RuleFor(fake => fake.ProductId, fake => productKey)
                        .RuleFor(fake => fake.SalespersonId, fake => salespersonKey)
                        .RuleFor(fake => fake.EndDate, fake => fake.Date.FutureOffset())
                        .RuleFor(fake => fake.DiscountPercentage, fake => fake.Random.Decimal((decimal)0, (decimal)0.3))
                        );
                }

                context.SaveChanges();
            }
        }
    }
}