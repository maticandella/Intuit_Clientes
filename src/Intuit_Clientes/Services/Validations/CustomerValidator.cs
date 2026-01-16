using FluentValidation;
using Intuit_Clientes.Domain;
using Intuit_Clientes.Utils;

namespace Intuit_Clientes.Services.Validations
{
    public class CustomerValidator : AbstractValidator<int>
    {
        private readonly Customer? _customerInDb;
        private readonly OperationIntent _operation;
        public CustomerValidator(OperationIntent operation, Customer customerInDb) 
        {
            _operation = operation;
            _customerInDb = customerInDb;

            When(x => _operation != OperationIntent.Add, () =>
            {
                RuleFor(x => x)
                    .Must(x => _customerInDb != null)
                    .WithMessage("El cliente con el ID especificado no existe");
            });
        }
    }
}
