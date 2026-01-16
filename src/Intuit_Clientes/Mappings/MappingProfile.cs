using AutoMapper;
using Intuit_Clientes.Domain;
using Intuit_Clientes.DTO;

namespace Intuit_Clientes.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<CustomerCommandDTO, Customer>();
            CreateMap<CustomerCreateDTO, Customer>();
            CreateMap<CustomerUpdateDTO, Customer>();
        }
    }
}
