using ErrorOr;
using FluentValidation;
using MediatR;

namespace TransactionService.Application.Behaviors.Common
{
    public class ValidationErrorOrBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IErrorOr
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationErrorOrBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = results.SelectMany(r => r.Errors).Where(f => f is not null).ToList();
            if (failures.Count == 0)
                return await next();

            // Convertimos FluentValidation -> ErrorOr Errors
            var errors = failures
                .Select(f => Error.Validation(
                    //code: $"{typeof(TRequest).Name}.{f.PropertyName}.{f.ErrorCode}",
                    code: $"{f.PropertyName}",
                    description: f.ErrorMessage,
                    metadata: new Dictionary<string, object>
                    {
                        ["field"] = f.PropertyName
                    }))
                .ToList();

            return (dynamic)errors;
        }
    }
}
