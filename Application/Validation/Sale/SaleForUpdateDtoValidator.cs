namespace Application.Validation.Sale
{
    using Application.Dtos.Sale;

    public class SaleForUpdateDtoValidator: SaleForManipulationDtoValidator<SaleForUpdateDto>
    {
        public SaleForUpdateDtoValidator()
        {
            // add fluent validation rules that should only be run on update operations here
            //https://fluentvalidation.net/
        }
    }
}