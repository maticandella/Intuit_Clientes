using Intuit_Clientes.Domain;

namespace Intuit_Clientes.Data.Abstractions
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer?> GetByIdForOperationsAsync(int id);
        Task<Customer> Create(Customer command);
        Task<int?> Update(Customer command);
        Task<int?> Delete(Customer command);
        Task<IEnumerable<Customer>> SearchByNameWithSpAsync(string parameter);
    }
}
