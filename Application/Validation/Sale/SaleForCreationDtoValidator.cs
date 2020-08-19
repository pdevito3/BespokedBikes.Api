namespace Application.Validation.Sale
{
    using Application.Dtos.Sale;

    public class SaleForCreationDtoValidator: SaleForManipulationDtoValidator<SaleForCreationDto>
    {
        public SaleForCreationDtoValidator()
        {
            // add fluent validation rules that should only be run on creation operations here
            //https://fluentvalidation.net/
        }
    }
}