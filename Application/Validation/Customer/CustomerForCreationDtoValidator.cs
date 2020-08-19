namespace Application.Validation.Customer
{
    using Application.Dtos.Customer;

    public class CustomerForCreationDtoValidator: CustomerForManipulationDtoValidator<CustomerForCreationDto>
    {
        public CustomerForCreationDtoValidator()
        {
            // add fluent validation rules that should only be run on creation operations here
            //https://fluentvalidation.net/
        }
    }
}