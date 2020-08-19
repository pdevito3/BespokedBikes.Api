namespace Infrastructure.Persistence.Seeders
{

    using AutoBogus;
    using Domain.Entities;
    using Infrastructure.Persistence.Contexts;
    using System.Linq;

    public static class SaleSeeder
    {
        public static void SeedSampleSaleData(BespokedBikesDbContext context)
        {
            if (!context.Sales.Any())
            {
                context.Sales.Add(new AutoFaker<Sale>());
                context.Sales.Add(new AutoFaker<Sale>());
                context.Sales.Add(new AutoFaker<Sale>());

                context.SaveChanges();
            }
        }
    }
}