namespace Application.Validation.Sale
{
    using Application.Dtos.Sale;
    using FluentValidation;
    using System;

    public class SaleForManipulationDtoValidator<T> : AbstractValidator<T> where T : SaleForManipulationDto
    {
        public SaleForManipulationDtoValidator()
        {
            // add fluent validation rules that should be shared between creation and update operations here
            //https://fluentvalidation.net/
        }
    }
}