using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Api.Common
{
    public class ErrorOrHttp
    {
        public static IActionResult MapToProblem(ControllerBase controller, List<Error> errors)
        {
            // Caso 1: todos son Validation -> ValidationProblemDetails
            if (errors.All(e => e.Type == ErrorType.Validation))
            {
                var dict = errors
               .GroupBy(e => e.Code) // normalmente: PropertyName
               .ToDictionary(
                   g => g.Key,
                   g => g.Select(e => e.Description).ToArray());

                var vpd = new ValidationProblemDetails(dict)
                {
                    Title = "Errores de validación",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = controller.HttpContext.Request.Path,
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Detail = "Uno o más campos tienen errores"
                };

                vpd.Extensions["traceId"] = controller.HttpContext.TraceIdentifier;

                return new BadRequestObjectResult(vpd);
                //return ValidationProblem(controller, errors);
            }

            // Caso 2: errores “normales” -> ProblemDetails
            var first = errors[0];
            var status = MapStatusCode(first.Type);

            var pd = new ProblemDetails
            {
                Title = first.Description,
                Status = status,
                Instance = controller.HttpContext.Request.Path,
                Type = $"https://httpstatuses.com/{status}",
                Detail = "Uno o más errores ocurrieron durante la operación"
            };

            pd.Extensions["traceId"] = controller.HttpContext.TraceIdentifier;
            pd.Extensions["errors"] = errors.Select(e => new
            {
                e.Code,
                e.Description,
                Type = e.Type.ToString()
            });

            return new ObjectResult(pd) { StatusCode = status };


            //return StatusCode(status, pd);


            var validation = errors.Where(e => e.Type == ErrorType.Validation).ToList();
            //if (validation.Count > 0)
            //{
            //    var dict = validation
            //        .GroupBy(e => e.Metadata!.TryGetValue("field", out var f) ? f?.ToString() : e.Code)
            //        .ToDictionary(
            //            g => g.Key ?? "validation",
            //            g => g.Select(x => x.Description).Distinct().ToArray()
            //        );

            //    var vpd = new ValidationProblemDetails(dict)
            //    {
            //        Title = "Validation error",
            //        Status = StatusCodes.Status400BadRequest,
            //        Instance = controller.HttpContext?.Request?.Path.Value
            //    };

            //    vpd.Extensions["correlationId"] = controller.HttpContext?.TraceIdentifier;
            //    return controller.ValidationProblem(vpd);
            //}

            //var main = errors[0];
            //var status = MapStatusCode(main.Type);
            //var status = main.Type switch
            //{
            //    ErrorType.NotFound => StatusCodes.Status404NotFound,
            //    ErrorType.Conflict => StatusCodes.Status409Conflict,
            //    ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            //    ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            //    _ => StatusCodes.Status500InternalServerError
            //};

            //var pd = new ProblemDetails
            //{
            //    Title = main.Type.ToString(),
            //    Detail = main.Description,
            //    Status = status,
            //    Type = main.Code,
            //    Instance = controller.HttpContext?.Request?.Path.Value
            //};

            //pd.Extensions["correlationId"] = controller.HttpContext?.TraceIdentifier;
            //return controller.StatusCode(status, pd);
        }

        private static IActionResult ValidationProblem(ControllerBase controller, List<Error> errors)
        {
            var dict = errors
                .GroupBy(e => e.Code) // code = PropertyName
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Description).ToArray());

            var vpd = new ValidationProblemDetails(dict)
            {
                Title = "Errores de validación",
                Status = StatusCodes.Status400BadRequest,
                Instance = controller.HttpContext?.Request?.Path.Value,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Detail = "Uno o más campos tienen errores"
            };

            return new BadRequestObjectResult(vpd);
        }

        private static int MapStatusCode(ErrorType type) => type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest
        };

    }
}
