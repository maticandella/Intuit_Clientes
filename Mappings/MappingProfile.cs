using AutoMapper;
using Intuit_Entrevista.Domain;
using Intuit_Entrevista.DTO;

namespace Intuit_Entrevista.Mappings
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
