namespace Application.Mappings
{
    using Application.Dtos.Sale;
    using AutoMapper;
    using Domain.Entities;

    public class SaleProfile : Profile
    {
        public SaleProfile()
        {
            //createmap<to this, from this>
            CreateMap<Sale, SaleDto>()
                .ForMember(dest => dest.ProductDto, opt => opt.MapFrom(src => src.Product))
                .ForMember(dest => dest.CustomerDto, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.SalespersonDto, opt => opt.MapFrom(src => src.Salesperson))
                .ReverseMap();
            CreateMap<SaleForCreationDto, Sale>();
            CreateMap<SaleForUpdateDto, Sale>()
                .ReverseMap();
        }
    }
}