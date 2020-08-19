namespace Application.Validation.Salesperson
{
    using Application.Dtos.Salesperson;

    public class SalespersonForUpdateDtoValidator: SalespersonForManipulationDtoValidator<SalespersonForUpdateDto>
    {
        public SalespersonForUpdateDtoValidator()
        {
            // add fluent validation rules that should only be run on update operations here
            //https://fluentvalidation.net/
        }
    }
}