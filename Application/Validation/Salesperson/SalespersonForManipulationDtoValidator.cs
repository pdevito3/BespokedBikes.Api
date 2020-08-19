namespace Application.Validation.Salesperson
{
    using Application.Dtos.Salesperson;
    using FluentValidation;
    using System;

    public class SalespersonForManipulationDtoValidator<T> : AbstractValidator<T> where T : SalespersonForManipulationDto
    {
        public SalespersonForManipulationDtoValidator()
        {
            // add fluent validation rules that should be shared between creation and update operations here
            //https://fluentvalidation.net/
        }
    }
}