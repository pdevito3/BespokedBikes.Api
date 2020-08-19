namespace BespokedBikes.Api.Tests.Fakes.Customer
{
    using AutoBogus;
        using Application.Dtos.Customer;

    // or replace 'AutoFaker' with 'Faker' along with your own rules if you don't want all fields to be auto faked
    public class FakeCustomerDto : AutoFaker<CustomerDto>
    {
        public FakeCustomerDto()
        {
            // if you want default values on any of your properties (e.g. an int between a certain range or a date always in the past), you can add `RuleFor` lines describing those defaults
            //RuleFor(c => c.ExampleIntProperty, c => c.Random.Number(50, 100000));
            //RuleFor(c => c.ExampleDateProperty, c => c.Date.Past()); 
        }
    }
}