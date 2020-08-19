namespace Application.Validation.Discount
{
    using Application.Dtos.Discount;
    using FluentValidation;
    using System;

    public class DiscountForManipulationDtoValidator<T> : AbstractValidator<T> where T : DiscountForManipulationDto
    {
        public DiscountForManipulationDtoValidator()
        {
            // add fluent validation rules that should be shared between creation and update operations here
            //https://fluentvalidation.net/
        }
    }
}