using FluentValidation.Results;
using Intuit_Entrevista.DTO;
using OneOf;

namespace Intuit_Entrevista.Services.Abstractions
{
    public interface ICustomerService
    {
        Task<List<CustomerDTO>> GetCustomersAsync();
        Task<CustomerDTO> GetByIdAsync(int id);
        Task<int?> Create(CustomerCreateDTO commandDTO);
        Task<OneOf<int?, IList<ValidationFailure>>> Update(int id, CustomerUpdateDTO commandDTO);
        Task<OneOf<int?, IList<ValidationFailure>>> Delete(int id);
        Task<List<CustomerDTO>> Search(string param);
    }
}
