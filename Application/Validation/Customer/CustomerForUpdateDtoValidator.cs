namespace Application.Validation.Customer
{
    using Application.Dtos.Customer;

    public class CustomerForUpdateDtoValidator: CustomerForManipulationDtoValidator<CustomerForUpdateDto>
    {
        public CustomerForUpdateDtoValidator()
        {
            // add fluent validation rules that should only be run on update operations here
            //https://fluentvalidation.net/
        }
    }
}