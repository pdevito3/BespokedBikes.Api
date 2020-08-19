namespace Application.Validation.Customer
{
    using Application.Dtos.Customer;
    using FluentValidation;
    using System;

    public class CustomerForManipulationDtoValidator<T> : AbstractValidator<T> where T : CustomerForManipulationDto
    {
        public CustomerForManipulationDtoValidator()
        {
            // add fluent validation rules that should be shared between creation and update operations here
            //https://fluentvalidation.net/
        }
    }
}