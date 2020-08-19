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
                .ReverseMap();
            CreateMap<SaleForCreationDto, Sale>();
            CreateMap<SaleForUpdateDto, Sale>()
                .ReverseMap();
        }
    }
}