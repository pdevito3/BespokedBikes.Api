namespace Infrastructure.Persistence.Seeders
{

    using AutoBogus;
    using Domain.Entities;
    using Infrastructure.Persistence.Contexts;
    using System.Linq;

    public static class ProductSeeder
    {
        public static void SeedSampleProductData(BespokedBikesDbContext context)
        {
            if (!context.Products.Any())
            {
                context.Products.Add(new AutoFaker<Product>());
                context.Products.Add(new AutoFaker<Product>());
                context.Products.Add(new AutoFaker<Product>());

                context.SaveChanges();
            }
        }
    }
}