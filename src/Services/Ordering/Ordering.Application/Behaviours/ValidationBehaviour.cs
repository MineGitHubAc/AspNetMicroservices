using FluentValidation;
using MediatR;
using ValidationException = Ordering.Application.Exceptions.ValidationException;

namespace Ordering.Application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators??throw new ArgumentNullException(nameof(validators));
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(_validators.Select(s=>s.ValidateAsync(context,cancellationToken)));
                var failures = validationResults.SelectMany(s=>s.Errors).Where(s=>s!=null).ToList();
                if (failures.Count != 0)
                    throw new ValidationException(failures);
            }
            return await next();
        }
    }
}
