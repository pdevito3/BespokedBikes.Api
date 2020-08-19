namespace Application.Mappings
{
    using Application.Dtos.Salesperson;
    using AutoMapper;
    using Domain.Entities;

    public class SalespersonProfile : Profile
    {
        public SalespersonProfile()
        {
            //createmap<to this, from this>
            CreateMap<Salesperson, SalespersonDto>()
                .ReverseMap();
            CreateMap<SalespersonForCreationDto, Salesperson>();
            CreateMap<SalespersonForUpdateDto, Salesperson>()
                .ReverseMap();
        }
    }
}