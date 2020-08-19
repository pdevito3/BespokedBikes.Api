namespace Application.Mappings
{
    using Application.Dtos.Discount;
    using AutoMapper;
    using Domain.Entities;

    public class DiscountProfile : Profile
    {
        public DiscountProfile()
        {
            //createmap<to this, from this>
            CreateMap<Discount, DiscountDto>()
                .ReverseMap();
            CreateMap<DiscountForCreationDto, Discount>();
            CreateMap<DiscountForUpdateDto, Discount>()
                .ReverseMap();
        }
    }
}