namespace Application.Validation.Discount
{
    using Application.Dtos.Discount;

    public class DiscountForUpdateDtoValidator: DiscountForManipulationDtoValidator<DiscountForUpdateDto>
    {
        public DiscountForUpdateDtoValidator()
        {
            // add fluent validation rules that should only be run on update operations here
            //https://fluentvalidation.net/
        }
    }
}