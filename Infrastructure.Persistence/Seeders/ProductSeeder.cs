namespace Infrastructure.Persistence.Seeders
{

    using AutoBogus;
    using Bogus;
    using Domain.Entities;
    using Infrastructure.Persistence.Contexts;
    using System;
    using System.Linq;

    public static class ProductSeeder
    {
        public static void SeedSampleProductData(BespokedBikesDbContext context)
        {
            if (!context.Products.Any())
            {
                for (var eachCustomer = 1; eachCustomer <= 50; eachCustomer++)
                {
                    var amount = new Faker().Random.Number(0, 50000);
                    var discount = new Faker().Random.Decimal((decimal).7, (decimal).95);

                    context.Products.Add(new AutoFaker<Product>()
                        .RuleFor(fake => fake.Name, fake => fake.Commerce.Product())
                        .RuleFor(fake => fake.Manufacturer, fake => fake.Company.CompanyName())
                        .RuleFor(fake => fake.Style, fake => fake.Commerce.Color())
                        .RuleFor(fake => fake.SalePrice, amount)
                        .RuleFor(fake => fake.PurchasePrice, fake => Convert.ToInt32((amount*discount)))
                        .RuleFor(fake => fake.QuantityOnHand, fake => fake.Random.Number(5,500))
                        .RuleFor(fake => fake.CommissionPercentage, fake => Math.Round(fake.Random.Decimal((decimal)0.1, (decimal)0.3),2))
                        );
                }

                context.SaveChanges();
            }
        }
    }
}