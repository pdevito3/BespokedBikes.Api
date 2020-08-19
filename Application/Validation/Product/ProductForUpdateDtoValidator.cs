namespace Application.Validation.Product
{
    using Application.Dtos.Product;

    public class ProductForUpdateDtoValidator: ProductForManipulationDtoValidator<ProductForUpdateDto>
    {
        public ProductForUpdateDtoValidator()
        {
            // add fluent validation rules that should only be run on update operations here
            //https://fluentvalidation.net/
        }
    }
}