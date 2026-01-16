using FluentValidation;
using Intuit_Entrevista.Domain;
using Intuit_Entrevista.Utils;

namespace Intuit_Entrevista.Services.Validations
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
