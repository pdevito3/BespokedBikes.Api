namespace Application.Validation.Product
{
    using Application.Dtos.Product;

    public class ProductForCreationDtoValidator: ProductForManipulationDtoValidator<ProductForCreationDto>
    {
        public ProductForCreationDtoValidator()
        {
            // add fluent validation rules that should only be run on creation operations here
            //https://fluentvalidation.net/
        }
    }
}