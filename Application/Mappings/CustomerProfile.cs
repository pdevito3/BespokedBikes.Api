namespace Application.Mappings
{
    using Application.Dtos.Customer;
    using AutoMapper;
    using Domain.Entities;

    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            //createmap<to this, from this>
            CreateMap<Customer, CustomerDto>()
                .ReverseMap();
            CreateMap<CustomerForCreationDto, Customer>();
            CreateMap<CustomerForUpdateDto, Customer>()
                .ReverseMap();
        }
    }
}