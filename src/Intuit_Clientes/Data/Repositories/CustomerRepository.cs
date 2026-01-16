using Intuit_Clientes.Data.Abstractions;
using Intuit_Clientes.Domain;
using Microsoft.EntityFrameworkCore;

namespace Intuit_Clientes.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly Context _context;
        private readonly DbSet<Customer> _customers;
        public CustomerRepository(Context context)
        {
            _context = context;
            _customers = _context.Set<Customer>();
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _customers.ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id) => await _customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Customer> Create(Customer command)
        {
            await _customers.AddAsync(command);
            await _context.SaveChangesAsync();
            return command;
        }

        public async Task<int?> Update(Customer command)
        {
            _customers.Update(command);
            return await _context.SaveChangesAsync();
        }

        public async Task<int?> Delete(Customer command)
        {
            _customers.Remove(command);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Customer>> SearchByNameWithSpAsync(string parameter)
        {
            // Normalizar el término de búsqueda
            var normalizedParam = string.IsNullOrWhiteSpace(parameter) ? "" : parameter.Trim();

            return await _customers
                .FromSqlRaw("SELECT * FROM sp_search_clientes_by_name({0})", normalizedParam)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
