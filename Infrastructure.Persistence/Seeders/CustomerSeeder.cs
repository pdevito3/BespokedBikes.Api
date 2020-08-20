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
                for(var eachCustomer = 1; eachCustomer <= 50; eachCustomer++)
                {
                    context.Customers.Add(new AutoFaker<Customer>()
                        .RuleFor(fake => fake.FirstName, fake => fake.Name.FirstName())
                        .RuleFor(fake => fake.LastName, fake => fake.Name.LastName())
                        .RuleFor(fake => fake.PhoneNumber, fake => fake.Phone.PhoneNumber("###-###-####"))
                        .RuleFor(fake => fake.StartDate, fake => fake.Date.PastOffset())
                        .RuleFor(fake => fake.Address1, fake => fake.Address.StreetAddress())
                        .RuleFor(fake => fake.Address2, fake => fake.Address.SecondaryAddress())
                        .RuleFor(fake => fake.State, fake => fake.Address.StateAbbr())
                        .RuleFor(fake => fake.City, fake => fake.Address.City())
                        .RuleFor(fake => fake.PostalCode, fake => fake.Address.ZipCode())
                        );
                }

                context.SaveChanges();
            }
        }
    }
}