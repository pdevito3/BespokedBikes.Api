namespace Application.Validation.Discount
{
    using Application.Dtos.Discount;

    public class DiscountForCreationDtoValidator: DiscountForManipulationDtoValidator<DiscountForCreationDto>
    {
        public DiscountForCreationDtoValidator()
        {
            // add fluent validation rules that should only be run on creation operations here
            //https://fluentvalidation.net/
        }
    }
}