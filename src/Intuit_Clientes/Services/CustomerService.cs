using AutoMapper;
using FluentValidation.Results;
using Intuit_Clientes.Data.Abstractions;
using Intuit_Clientes.Domain;
using Intuit_Clientes.DTO;
using Intuit_Clientes.Services.Abstractions;
using Intuit_Clientes.Services.Validations;
using Intuit_Clientes.Utils;
using OneOf;

namespace Intuit_Clientes.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;
        public CustomerService(IMapper mapper, ICustomerRepository customerRepository)
        {
            _mapper = mapper;
            _customerRepository = customerRepository;
        }

        public async Task<List<CustomerDTO>> GetCustomersAsync()
        {
            //TODO controlar errores
            var result = await _customerRepository.GetAllAsync();
            return _mapper.Map<List<CustomerDTO>>(result);
        }

        public async Task<CustomerDTO> GetByIdAsync(int id)
        {
            //TODO controlar errores
            var result = await _customerRepository.GetByIdAsync(id);
            return _mapper.Map<CustomerDTO>(result);
        }

        public async Task<int?> Create(CustomerCreateDTO commandDTO)
        {
            //TODO controlar errores
            var command = _mapper.Map<Customer>(commandDTO);
            var created = await _customerRepository.Create(command);
            return created?.Id;
        }

        public async Task<OneOf<int?, IList<ValidationFailure>>> Update(int id, CustomerUpdateDTO commandDTO)
        {
            //TODO controlar errores
            var customerInDb = await _customerRepository.GetByIdAsync(id);

            var validator = new CustomerValidator(OperationIntent.Update, customerInDb);

            var validationResult = await validator.ValidateAsync(id);

            if (!validationResult.IsValid)
                return validationResult.Errors.Select(e => new ValidationFailure(e.PropertyName, e.ErrorMessage)).ToList();

            //TODO esto lo puedo resolver mucho mejor con automapper
            customerInDb.Nombre = commandDTO.Nombre;
            customerInDb.Apellido = commandDTO.Apellido;
            customerInDb.RazonSocial = commandDTO.RazonSocial;
            customerInDb.CUIT = commandDTO.CUIT;
            customerInDb.FechaNacimiento = commandDTO.FechaNacimiento;
            customerInDb.TelefonoCelular = commandDTO.TelefonoCelular;
            customerInDb.Email = commandDTO.Email;
            customerInDb.FechaModificacion = DateTime.Now;

            return await _customerRepository.Update(customerInDb);
        }

        public async Task<OneOf<int?, IList<ValidationFailure>>> Delete(int id)
        {
            var customerInDb = await _customerRepository.GetByIdAsync(id);

            var validator = new CustomerValidator(OperationIntent.Delete, customerInDb);

            var validationResult = await validator.ValidateAsync(id);

            if (!validationResult.IsValid)
                return validationResult.Errors.Select(e => new ValidationFailure(e.PropertyName, e.ErrorMessage)).ToList();

            return await _customerRepository.Delete(customerInDb);
        }

        public async Task<List<CustomerDTO>> Search(string param)
        {
            var customers = await _customerRepository.SearchByNameWithSpAsync(param);
            return _mapper.Map<List<CustomerDTO>>(customers);
        }
    }
}
