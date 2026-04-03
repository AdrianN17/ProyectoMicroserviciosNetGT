using CustomerService.Application.Common.Exceptions;
using CustomerService.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CustomerService.Api.Common
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        //private readonly IProblemDetailsService _problemDetails;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        //public GlobalExceptionHandler(IProblemDetailsService problemDetails, ILogger<GlobalExceptionHandler> logger)
        //    => (_problemDetails, _logger) = (problemDetails, logger);

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => (_logger) = (logger);

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled exception");


            ProblemDetails problem;

            switch (exception)
            {
                case ValidationException vex:
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                        var errors = vex.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.ErrorMessage).Distinct().ToArray()
                            );

                        problem = new ValidationProblemDetails(errors)
                        {
                            Title = "Validation failed",
                            Status = StatusCodes.Status400BadRequest,
                            // Si quieres, usa el RFC de 400:
                            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                            Detail = "Uno o más campos tienen errores."
                        };
                        break;
                    }
                case BusinessRuleViolationException bex:
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

                        //var errors = bex.Errors
                        //    .GroupBy(e => e.Key)
                        //    .ToDictionary(
                        //        g => g.Key,
                        //        g => g.SelectMany(e => e.Value).Distinct().ToArray()
                        //    );

                        problem = new ProblemDetails()
                        {
                            Title = "Domain validation failed",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                            Detail = bex.Message
                        };
                        problem.Extensions["code"] = bex.Code;
                        break;
                    }
                case DomainValidationException dex:
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

                        var errors = dex.Errors
                            .GroupBy(e => e.Key)
                            .ToDictionary(
                                g => g.Key,
                                g => g.SelectMany(e => e.Value).Distinct().ToArray()
                            );

                        problem = new ValidationProblemDetails(errors)
                        {
                            Title = "Domain validation failed",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                            Detail = "La operación no se pudo completar debido a errores de validación en el dominio."
                        };
                        problem.Extensions["code"] = dex.Code;
                        break;
                    }
                case NotFoundException nfex:
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

                        problem = new ProblemDetails
                        {
                            Title = "Not found",
                            Status = StatusCodes.Status404NotFound,
                            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4",
                            Detail = nfex.Message
                        };
                        problem.Extensions["code"] = nfex.Code;
                        break;
                    }
                default:
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        problem = new ProblemDetails
                        {
                            Title = "Unexpected error",
                            Status = StatusCodes.Status500InternalServerError,
                            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                            Detail = "Ocurrió un error inesperado."
                        };
                        break;
                    }
            }

            // IDs útiles (sin filtrar información sensible)
            problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var corr) && !string.IsNullOrWhiteSpace(corr))
                problem.Extensions["correlationId"] = corr.ToString();

            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync((object)problem, cancellationToken);

            return true; // ya lo manejamos
        }
    }
}
