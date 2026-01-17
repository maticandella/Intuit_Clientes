using AutoMapper;
using Intuit_Clientes.Data.Abstractions;
using Intuit_Clientes.Domain;
using Intuit_Clientes.DTO;
using Intuit_Clientes.Services;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Intuit_Clientes.Mappings;
using Castle.Core.Resource;

namespace Intuit_Clientes.Tests.Services
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private IMapper _mapper;
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private CustomerService _customerService;
        private Mock<ILogger<CustomerService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();

            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger<CustomerService>>();

            // 2. Inyectamos el Mapper REAL
            _customerService = new CustomerService(
                _mapper,
                _customerRepositoryMock.Object,
                _loggerMock.Object);
        }

        #region GetCustomersAsync Tests

        [Test]
        public async Task GetCustomersAsync_WhenCalled_ReturnsListOfCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Nombre = "Juan", Apellido = "Pérez" },
                new Customer { Id = 2, Nombre = "María", Apellido = "González" }
            };

            var customersDTO = new List<CustomerDTO>
            {
                new CustomerDTO { Id = 1, Nombre = "Juan", Apellido = "Pérez" },
                new CustomerDTO { Id = 2, Nombre = "María", Apellido = "González" }
            };

            _customerRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(customers);

            // Act
            var result = await _customerService.GetCustomersAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Nombre, Is.EqualTo("Juan"));
            Assert.That(result[1].Nombre, Is.EqualTo("María"));
            _customerRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetCustomersAsync_WhenNoCustomers_ReturnsEmptyList()
        {
            // Arrange
            var emptyCustomers = new List<Customer>();
            var emptyCustomersDTO = new List<CustomerDTO>();

            _customerRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(emptyCustomers);

            // Act
            var result = await _customerService.GetCustomersAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            _customerRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Test]
        public void GetCustomersAsync_WhenRepositoryThrowsException_ThrowsException()
        {
            // Arrange
            _customerRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _customerService.GetCustomersAsync());
        }

        #endregion

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_WithValidId_ReturnsCustomer()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer
            {
                Id = customerId,
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan@example.com"
            };

            var customerDTO = new CustomerDTO
            {
                Id = customerId,
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan@example.com"
            };

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(customerId))
                .ReturnsAsync(customer);

            // Act
            var result = await _customerService.GetByIdAsync(customerId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(customerId));
            Assert.That(result.Nombre, Is.EqualTo("Juan"));
            _customerRepositoryMock.Verify(x => x.GetByIdAsync(customerId), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var invalidId = 999;

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(invalidId))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _customerService.GetByIdAsync(invalidId);

            // Assert
            Assert.That(result, Is.Null);
            _customerRepositoryMock.Verify(x => x.GetByIdAsync(invalidId), Times.Once);
        }

        [Test]
        public void GetByIdAsync_WhenRepositoryThrowsException_ThrowsException()
        {
            // Arrange
            var customerId = 1;
            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(customerId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _customerService.GetByIdAsync(customerId));
        }

        #endregion

        #region Create Tests

        [Test]
        public async Task Create_WithValidData_ReturnsCustomerId()
        {
            // Arrange
            var customerCreateDTO = new CustomerCreateDTO
            {
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan@example.com",
                CUIT = "20-12345678-9"
            };

            var createdCustomer = new Customer
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Pérez"
            };

            _customerRepositoryMock
                .Setup(x => x.Create(It.IsAny<Customer>()))
                .ReturnsAsync(createdCustomer);

            // Act
            var result = await _customerService.Create(customerCreateDTO);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public async Task Create_WithValidData_MapsCorrectly()
        {
            // Arrange
            var customerCreateDTO = new CustomerCreateDTO
            {
                Nombre = "María",
                Apellido = "González",
                Email = "maria@example.com",
                TelefonoCelular = "1234567890"
            };

            var customer = new Customer();
            var createdCustomer = new Customer { Id = 2 };

            _customerRepositoryMock
                .Setup(x => x.Create(It.IsAny<Customer>()))
                .ReturnsAsync(createdCustomer);

            // Act
            var result = await _customerService.Create(customerCreateDTO);

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task Create_WhenRepositoryReturnsNull_ReturnsNull()
        {
            // Arrange
            var customerCreateDTO = new CustomerCreateDTO
            {
                Nombre = "Test",
                Apellido = "User"
            };

            var customer = new Customer();

            _customerRepositoryMock
                .Setup(x => x.Create(customer))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _customerService.Create(customerCreateDTO);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Create_WhenRepositoryThrowsException_ThrowsException()
        {
            // Arrange
            var customerCreateDTO = new CustomerCreateDTO
            {
                Nombre = "Test",
                Apellido = "User"
            };

            _customerRepositoryMock
                .Setup(x => x.Create(It.IsAny<Customer>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _customerService.Create(customerCreateDTO));
        }

        #endregion

        #region Update Tests

        [Test]
        public async Task Update_WithValidData_ReturnsCustomerId()
        {
            // Arrange
            var customerId = 1;
            var customerUpdateDTO = new CustomerUpdateDTO
            {
                Nombre = "Juan Actualizado",
                Apellido = "Pérez",
                Email = "juan.nuevo@example.com",
                CUIT = "20-12345678-9"
            };

            var existingCustomer = new Customer
            {
                Id = customerId,
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan@example.com",
                CUIT = "20-12345678-9",
                FechaCreacion = DateTime.Now.AddMonths(-1)
            };

            _customerRepositoryMock
                .Setup(x => x.GetByIdForOperationsAsync(customerId))
                .ReturnsAsync(existingCustomer);

            _customerRepositoryMock
                .Setup(x => x.Update(It.IsAny<Customer>()))
                .ReturnsAsync(customerId);

            // Act
            var result = await _customerService.Update(customerId, customerUpdateDTO);

            // Assert
            Assert.That(result.IsT0, Is.True);
            Assert.That(result.AsT0, Is.EqualTo(customerId));
            Assert.That(existingCustomer.Nombre, Is.EqualTo(customerUpdateDTO.Nombre));
            Assert.That(existingCustomer.Email, Is.EqualTo(customerUpdateDTO.Email));
            _customerRepositoryMock.Verify(x => x.GetByIdForOperationsAsync(customerId), Times.Once);
            _customerRepositoryMock.Verify(x => x.Update(existingCustomer), Times.Once);
        }

        [Test]
        public async Task Update_UpdatesAllFields_Correctly()
        {
            // Arrange
            var customerId = 1;
            var customerUpdateDTO = new CustomerUpdateDTO
            {
                Nombre = "Nuevo Nombre",
                Apellido = "Nuevo Apellido",
                RazonSocial = "Nueva Razón Social",
                CUIT = "20-98765432-1",
                FechaNacimiento = new DateTime(1990, 5, 15),
                TelefonoCelular = "9876543210",
                Email = "nuevo@example.com"
            };

            var existingCustomer = new Customer
            {
                Id = customerId,
                Nombre = "Viejo",
                Apellido = "Apellido",
                Email = "viejo@example.com"
            };

            _customerRepositoryMock
                .Setup(x => x.GetByIdForOperationsAsync(customerId))
                .ReturnsAsync(existingCustomer);

            _customerRepositoryMock
                .Setup(x => x.Update(It.IsAny<Customer>()))
                .ReturnsAsync(customerId);

            // Act
            var result = await _customerService.Update(customerId, customerUpdateDTO);

            // Assert
            Assert.That(result.IsT0, Is.True);
            Assert.That(existingCustomer.Nombre, Is.EqualTo(customerUpdateDTO.Nombre));
            Assert.That(existingCustomer.Apellido, Is.EqualTo(customerUpdateDTO.Apellido));
            Assert.That(existingCustomer.RazonSocial, Is.EqualTo(customerUpdateDTO.RazonSocial));
            Assert.That(existingCustomer.CUIT, Is.EqualTo(customerUpdateDTO.CUIT));
            Assert.That(existingCustomer.FechaNacimiento, Is.EqualTo(customerUpdateDTO.FechaNacimiento));
            Assert.That(existingCustomer.TelefonoCelular, Is.EqualTo(customerUpdateDTO.TelefonoCelular));
            Assert.That(existingCustomer.Email, Is.EqualTo(customerUpdateDTO.Email));
            Assert.That(existingCustomer.FechaModificacion, Is.Not.Null);
        }

        [Test]
        public async Task Update_WithNonExistentCustomer_ReturnsValidationErrors()
        {
            // Arrange
            var customerId = 999;
            var customerUpdateDTO = new CustomerUpdateDTO
            {
                Nombre = "Test",
                Apellido = "User"
            };

            _customerRepositoryMock
                .Setup(x => x.GetByIdForOperationsAsync(customerId))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _customerService.Update(customerId, customerUpdateDTO);

            // Assert
            Assert.That(result.IsT1, Is.True);
            var errors = result.AsT1;
            Assert.That(errors, Is.Not.Empty);
            Assert.That(errors.Any(e => e.ErrorMessage.Contains("no existe")), Is.True);
            _customerRepositoryMock.Verify(x => x.GetByIdForOperationsAsync(customerId), Times.Once);
            _customerRepositoryMock.Verify(x => x.Update(It.IsAny<Customer>()), Times.Never);
        }

        [Test]
        public async Task Update_SetsFechaModificacion_ToCurrentDateTime()
        {
            // Arrange
            var customerId = 1;
            var customerUpdateDTO = new CustomerUpdateDTO
            {
                Nombre = "Test",
                Apellido = "User"
            };

            var existingCustomer = new Customer
            {
                Id = customerId,
                Nombre = "Old",
                Apellido = "Name",
                FechaCreacion = DateTime.Now.AddMonths(-1),
                FechaModificacion = null
            };

            var beforeUpdate = DateTime.Now;

            _customerRepositoryMock
                .Setup(x => x.GetByIdForOperationsAsync(customerId))
                .ReturnsAsync(existingCustomer);

            _customerRepositoryMock
                .Setup(x => x.Update(It.IsAny<Customer>()))
                .ReturnsAsync(customerId);

            // Act
            var result = await _customerService.Update(customerId, customerUpdateDTO);
            var afterUpdate = DateTime.Now;

            // Assert
            Assert.That(result.IsT0, Is.True);
            Assert.That(existingCustomer.FechaModificacion, Is.Not.Null);
            Assert.That(existingCustomer.FechaModificacion.Value, Is.GreaterThanOrEqualTo(beforeUpdate));
            Assert.That(existingCustomer.FechaModificacion.Value, Is.LessThanOrEqualTo(afterUpdate));
        }

        [Test]
        public void Update_WhenRepositoryThrowsException_ThrowsException()
        {
            // Arrange
            var customerId = 1;
            var customerUpdateDTO = new CustomerUpdateDTO
            {
                Nombre = "Test",
                Apellido = "User"
            };

            var existingCustomer = new Customer { Id = customerId };

            _customerRepositoryMock
                .Setup(x => x.GetByIdForOperationsAsync(customerId))
                .ReturnsAsync(existingCustomer);

            _customerRepositoryMock
                .Setup(x => x.Update(It.IsAny<Customer>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _customerService.Update(customerId, customerUpdateDTO));
        }

        #endregion

        #region Delete Tests

        [Test]
        public async Task Delete_WithValidId_ReturnsCustomerId()
        {
            // Arrange
            var customerId = 1;
            var existingCustomer = new Customer
            {
                Id = customerId,
                Nombre = "Juan",
                Apellido = "Pérez"
            };

            _customerRepositoryMock
                .Setup(x => x.GetByIdForOperationsAsync(customerId))
                .ReturnsAsync(existingCustomer);

            _customerRepositoryMock
                .Setup(x => x.Delete(existingCustomer))
                .ReturnsAsync(customerId);

            // Act
            var result = await _customerService.Delete(customerId);

            // Assert
            Assert.That(result.IsT0, Is.True);
            Assert.That(result.AsT0, Is.EqualTo(customerId));
            _customerRepositoryMock.Verify(x => x.GetByIdForOperationsAsync(customerId), Times.Once);
            _customerRepositoryMock.Verify(x => x.Delete(existingCustomer), Times.Once);
        }

        [Test]
        public async Task Delete_WithNonExistentCustomer_ReturnsValidationErrors()
        {
            // Arrange
            var customerId = 999;

            _customerRepositoryMock
                .Setup(x => x.GetByIdForOperationsAsync(customerId))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _customerService.Delete(customerId);

            // Assert
            Assert.That(result.IsT1, Is.True);
            var errors = result.AsT1;
            Assert.That(errors, Is.Not.Empty);
            Assert.That(errors.Any(e => e.ErrorMessage.Contains("no existe")), Is.True);
            _customerRepositoryMock.Verify(x => x.GetByIdForOperationsAsync(customerId), Times.Once);
            _customerRepositoryMock.Verify(x => x.Delete(It.IsAny<Customer>()), Times.Never);
        }

        [Test]
        public async Task Delete_CallsRepositoryWithCorrectCustomer()
        {
            // Arrange
            var customerId = 1;
            var existingCustomer = new Customer
            {
                Id = customerId,
                Nombre = "Test",
                Apellido = "User"
            };

            Customer capturedCustomer = null;

            _customerRepositoryMock
                .Setup(x => x.GetByIdForOperationsAsync(customerId))
                .ReturnsAsync(existingCustomer);

            _customerRepositoryMock
                .Setup(x => x.Delete(It.IsAny<Customer>()))
                .Callback<Customer>(c => capturedCustomer = c)
                .ReturnsAsync(customerId);

            // Act
            await _customerService.Delete(customerId);

            // Assert
            Assert.That(capturedCustomer, Is.Not.Null);
            Assert.That(capturedCustomer.Id, Is.EqualTo(customerId));
            Assert.That(capturedCustomer.Nombre, Is.EqualTo("Test"));
        }

        [Test]
        public void Delete_WhenRepositoryThrowsException_ThrowsException()
        {
            // Arrange
            var customerId = 1;
            var existingCustomer = new Customer { Id = customerId };

            _customerRepositoryMock
                .Setup(x => x.GetByIdForOperationsAsync(customerId))
                .ReturnsAsync(existingCustomer);

            _customerRepositoryMock
                .Setup(x => x.Delete(It.IsAny<Customer>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _customerService.Delete(customerId));
        }

        #endregion

        #region Search Tests

        [Test]
        public async Task Search_WithValidParam_ReturnsMatchingCustomers()
        {
            // Arrange
            var searchParam = "Juan";
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Nombre = "Juan", Apellido = "Pérez" },
                new Customer { Id = 2, Nombre = "Juan", Apellido = "García" }
            };

            var customersDTO = new List<CustomerDTO>
            {
                new CustomerDTO { Id = 1, Nombre = "Juan", Apellido = "Pérez" },
                new CustomerDTO { Id = 2, Nombre = "Juan", Apellido = "García" }
            };

            _customerRepositoryMock
                .Setup(x => x.SearchByNameWithSpAsync(searchParam))
                .ReturnsAsync(customers);

            // Act
            var result = await _customerService.Search(searchParam);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(c => c.Nombre == "Juan"), Is.True);
            _customerRepositoryMock.Verify(x => x.SearchByNameWithSpAsync(searchParam), Times.Once);
        }

        [Test]
        public async Task Search_WithNoMatches_ReturnsEmptyList()
        {
            // Arrange
            var searchParam = "NoExiste";
            var emptyCustomers = new List<Customer>();
            var emptyCustomersDTO = new List<CustomerDTO>();

            _customerRepositoryMock
                .Setup(x => x.SearchByNameWithSpAsync(searchParam))
                .ReturnsAsync(emptyCustomers);

            // Act
            var result = await _customerService.Search(searchParam);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            _customerRepositoryMock.Verify(x => x.SearchByNameWithSpAsync(searchParam), Times.Once);
        }

        [Test]
        public async Task Search_WithEmptyString_CallsRepository()
        {
            // Arrange
            var searchParam = "";
            var customers = new List<Customer>();
            var customersDTO = new List<CustomerDTO>();

            _customerRepositoryMock
                .Setup(x => x.SearchByNameWithSpAsync(searchParam))
                .ReturnsAsync(customers);

            // Act
            var result = await _customerService.Search(searchParam);

            // Assert
            Assert.That(result, Is.Not.Null);
            _customerRepositoryMock.Verify(x => x.SearchByNameWithSpAsync(searchParam), Times.Once);
        }

        [Test]
        public async Task Search_MapsResultsCorrectly()
        {
            // Arrange
            var searchParam = "Pérez";
            var customers = new List<Customer>
            {
                new Customer
                {
                    Id = 1,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Email = "juan@example.com"
                }
            };

            var customersDTO = new List<CustomerDTO>
            {
                new CustomerDTO
                {
                    Id = 1,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Email = "juan@example.com"
                }
            };

            _customerRepositoryMock
                .Setup(x => x.SearchByNameWithSpAsync(searchParam))
                .ReturnsAsync(customers);

            // Act
            var result = await _customerService.Search(searchParam);

            // Assert
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].Nombre, Is.EqualTo("Juan"));
            Assert.That(result[0].Email, Is.EqualTo("juan@example.com"));
        }

        [Test]
        public void Search_WhenRepositoryThrowsException_ThrowsException()
        {
            // Arrange
            var searchParam = "Test";
            _customerRepositoryMock
                .Setup(x => x.SearchByNameWithSpAsync(searchParam))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _customerService.Search(searchParam));
        }

        #endregion

        #region Integration/Edge Cases Tests

        [Test]
        public async Task Update_PreservesExistingIdAndCreationDate()
        {
            // Arrange
            var customerId = 1;
            var originalCreationDate = DateTime.Now.AddYears(-1);
            var customerUpdateDTO = new CustomerUpdateDTO
            {
                Nombre = "Nuevo",
                Apellido = "Nombre"
            };

            var existingCustomer = new Customer
            {
                Id = customerId,
                Nombre = "Viejo",
                Apellido = "Apellido",
                FechaCreacion = originalCreationDate
            };

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(customerId))
                .ReturnsAsync(existingCustomer);

            _customerRepositoryMock
                .Setup(x => x.Update(It.IsAny<Customer>()))
                .ReturnsAsync(customerId);

            // Act
            await _customerService.Update(customerId, customerUpdateDTO);

            // Assert
            Assert.That(existingCustomer.Id, Is.EqualTo(customerId));
            Assert.That(existingCustomer.FechaCreacion, Is.EqualTo(originalCreationDate));
        }

        [Test]
        public async Task Create_WithNullDTO_ReturnsNull()
        {
            // Arrange
            CustomerCreateDTO nullDTO = null;

            _customerRepositoryMock
                .Setup(x => x.Create(It.IsAny<Customer>()))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _customerService.Create(nullDTO);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByIdAsync_WithZeroId_CallsRepository()
        {
            // Arrange
            var customerId = 0;

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(customerId))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _customerService.GetByIdAsync(customerId);

            // Assert
            Assert.That(result, Is.Null);
            _customerRepositoryMock.Verify(x => x.GetByIdAsync(customerId), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_WithNegativeId_CallsRepository()
        {
            // Arrange
            var customerId = -1;

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(customerId))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _customerService.GetByIdAsync(customerId);

            // Assert
            _customerRepositoryMock.Verify(x => x.GetByIdAsync(customerId), Times.Once);
        }

        #endregion
    }
}