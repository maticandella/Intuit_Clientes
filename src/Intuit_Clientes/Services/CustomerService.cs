using AutoMapper;
using FluentValidation.Results;
using Intuit_Clientes.Data.Abstractions;
using Intuit_Clientes.Domain;
using Intuit_Clientes.DTO;
using Intuit_Clientes.Services.Abstractions;
using Intuit_Clientes.Services.Validations;
using Intuit_Clientes.Utils;
using OneOf;
using System.Diagnostics;

namespace Intuit_Clientes.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerService> _logger;
        public CustomerService(IMapper mapper, ICustomerRepository customerRepository, ILogger<CustomerService> logger)
        {
            _mapper = mapper;
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<List<CustomerDTO>> GetCustomersAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("Obteniendo lista completa de clientes");
                var result = await _customerRepository.GetAllAsync();
                var mappedResult = _mapper.Map<List<CustomerDTO>>(result);

                _logger.LogInformation(
                    "Se obtuvieron {Count} clientes exitosamente en {ElapsedMs}ms",
                    mappedResult.Count,
                    stopwatch.ElapsedMilliseconds);

                return mappedResult;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    "Error al obtener la lista de clientes. Tiempo transcurrido: {ElapsedMs}ms",
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<CustomerDTO> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Buscando al cliente con ID: {id}", id);
                var result = await _customerRepository.GetByIdAsync(id);
                _logger.LogInformation("Cliente obtenido con éxito");
                return _mapper.Map<CustomerDTO>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al obtener datos del cliente con ID: {id}",
                    id);
                throw;
            }
            
        }

        public async Task<int?> Create(CustomerCreateDTO commandDTO)
        {
            if (commandDTO == null)
            {
                _logger.LogWarning("Se intentó crear un cliente con datos nulos");
                return null;
            }
            try
            {
                _logger.LogInformation("Creando cliente con CUIT: {cuit}", commandDTO.CUIT);
                var command = _mapper.Map<Customer>(commandDTO);
                var created = await _customerRepository.Create(command);

                _logger.LogInformation("Cliente creado con éxito con ID: {id}", created?.Id);
                return created?.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al crear al cliente con CUIT: {cuit}",
                    commandDTO.CUIT);
                throw;
            }
        }

        public async Task<OneOf<int?, IList<ValidationFailure>>> Update(int id, CustomerUpdateDTO commandDTO)
        {
            try
            {
                _logger.LogInformation("Modificando al cliente con ID: {id}", id);
                var customerInDb = await _customerRepository.GetByIdForOperationsAsync(id);

                var validator = new CustomerValidator(OperationIntent.Update, customerInDb);

                var validationResult = await validator.ValidateAsync(id);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("El cliente con el ID especificado no existe");
                    return validationResult.Errors.Select(e => new ValidationFailure(e.PropertyName, e.ErrorMessage)).ToList();
                }

                _mapper.Map(commandDTO, customerInDb);
                customerInDb.FechaModificacion = DateTime.Now;

                var result = await _customerRepository.Update(customerInDb);
                _logger.LogInformation("Cliente modificado con éxito");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al modificar al cliente con ID: {id}",
                    id);
                throw;
            }
        }

        public async Task<OneOf<int?, IList<ValidationFailure>>> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando al cliente con ID: {id}", id);
                var customerInDb = await _customerRepository.GetByIdForOperationsAsync(id);

                var validator = new CustomerValidator(OperationIntent.Delete, customerInDb);

                var validationResult = await validator.ValidateAsync(id);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("El cliente con el ID especificado no existe");
                    return validationResult.Errors.Select(e => new ValidationFailure(e.PropertyName, e.ErrorMessage)).ToList();
                }

                var result = await _customerRepository.Delete(customerInDb);
                _logger.LogInformation("Cliente eliminado con éxito");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al eliminar al cliente con ID: {id}",
                    id);
                throw;
            }
        }

        public async Task<List<CustomerDTO>> Search(string param)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("Iniciando búsqueda de clientes con el parámetro: {Param}", param);
                var customers = await _customerRepository.SearchByNameWithSpAsync(param);

                if (customers == null || !customers.Any())
                {
                    _logger.LogWarning("No se encontraron clientes para la búsqueda: {Param}", param);
                    return new List<CustomerDTO>();
                }

                var mappedResult = _mapper.Map<List<CustomerDTO>>(customers);

                _logger.LogInformation(
                    "Búsqueda finalizada exitosamente. Se encontraron {Count} resultados en {ElapsedMs}ms",
                    mappedResult.Count,
                    stopwatch.ElapsedMilliseconds);

                return mappedResult;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error durante la búsqueda con el parámetro: {Param}", param);
                throw;
            }
        }
    }
}
