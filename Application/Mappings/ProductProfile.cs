namespace Application.Mappings
{
    using Application.Dtos.Product;
    using AutoMapper;
    using Domain.Entities;

    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            //createmap<to this, from this>
            CreateMap<Product, ProductDto>()
                .ReverseMap();
            CreateMap<ProductForCreationDto, Product>();
            CreateMap<ProductForUpdateDto, Product>()
                .ReverseMap();
        }
    }
}