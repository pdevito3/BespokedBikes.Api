namespace Application.Validation.Salesperson
{
    using Application.Dtos.Salesperson;

    public class SalespersonForCreationDtoValidator: SalespersonForManipulationDtoValidator<SalespersonForCreationDto>
    {
        public SalespersonForCreationDtoValidator()
        {
            // add fluent validation rules that should only be run on creation operations here
            //https://fluentvalidation.net/
        }
    }
}