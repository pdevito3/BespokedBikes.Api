namespace Application.Validation.Product
{
    using Application.Dtos.Product;
    using FluentValidation;
    using System;

    public class ProductForManipulationDtoValidator<T> : AbstractValidator<T> where T : ProductForManipulationDto
    {
        public ProductForManipulationDtoValidator()
        {
            // add fluent validation rules that should be shared between creation and update operations here
            //https://fluentvalidation.net/
        }
    }
}