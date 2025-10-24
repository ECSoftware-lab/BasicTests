using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace TestsFilters.Filters
{
    public class ValidationActionFilter : IActionFilter
    {
        private readonly ILogger<ValidationActionFilter> _logger;
        private const string StopwatchKey = "ActionStopwatch";
        public ValidationActionFilter(ILogger<ValidationActionFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var actionName = context.ActionDescriptor.DisplayName;
            var controllerName = context.Controller.GetType().Name;
            _logger.LogInformation(
            "Ejecutando Action: {Controller}.{Action}",
            controllerName,
            actionName);

            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                _logger.LogWarning(
                    "ModelState inválido para {Action}. Errores: {Errors}",
                    actionName,
                    string.Join(", ", errors.SelectMany(e => e.Value ?? Array.Empty<string>())));
                 
                context.Result = new BadRequestObjectResult(new
                {
                    success = false,
                    message = "Errores de validación",
                    errors = errors,
                    timestamp = DateTime.UtcNow
                });

                return; // corta el pipeline 
            }
            var nullParameters = context.ActionArguments
            .Where(arg => arg.Value == null)
            .Select(arg => arg.Key)
            .ToList();

            if (nullParameters.Any())
            {
                _logger.LogWarning(
                    "Parámetros null detectados en {Action}: {Parameters}",
                    actionName,
                    string.Join(", ", nullParameters));

                context.Result = new BadRequestObjectResult(new
                {
                    success = false,
                    message = "Parámetros requeridos son null",
                    nullParameters = nullParameters,
                    timestamp = DateTime.UtcNow
                });

                return;
            }

            var stopwatch = Stopwatch.StartNew();
            context.HttpContext.Items[StopwatchKey] = stopwatch;

            if (context.ActionArguments.Any())
            {
                var parameters = string.Join(", ",
                    context.ActionArguments.Select(arg =>
                        $"{arg.Key}={arg.Value}"));

                _logger.LogDebug(
                    "Parámetros de entrada: {Parameters}",
                    parameters);
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var actionName = context.ActionDescriptor.DisplayName;
             Stopwatch? stopwatch = null;

            if (context.HttpContext.Items[StopwatchKey] is Stopwatch sw)
            {
                sw.Stop();
                stopwatch = sw;
                var executionTime = stopwatch.ElapsedMilliseconds;
                 
                context.HttpContext.Response.Headers["X-Execution-Time-Ms"] =
                    executionTime.ToString();

                _logger.LogInformation(
                    "Action {Action} ejecutado en {ExecutionTime}ms",
                    actionName,
                    executionTime);
                 
                if (executionTime > 1000)
                {
                    _logger.LogWarning(
                        "Action LENTO detectado: {Action} tardó {ExecutionTime}ms",
                        actionName,
                        executionTime);
                }
            }
             
            if (context.Exception != null && !context.ExceptionHandled)
            {
                _logger.LogError(
                    context.Exception,
                    "Excepción no controlada en {Action}: {Message}",
                    actionName,
                    context.Exception.Message);
                 
                context.ExceptionHandled = true;
                 
                context.Result = new ObjectResult(new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = context.Exception.Message,
                    timestamp = DateTime.UtcNow
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
             
            if (context.Result is ObjectResult objectResult &&
                objectResult.StatusCode == 200)
            {
                var originalValue = objectResult.Value;

                var wrappedResult = new ObjectResult(new
                {
                    success = true,
                    data = originalValue,
                    timestamp = DateTime.UtcNow,
                    executionTimeMs = stopwatch?.ElapsedMilliseconds ?? 0
                })
                {
                    StatusCode = objectResult.StatusCode,
                    DeclaredType = objectResult.DeclaredType
                };

                context.Result = wrappedResult;

                _logger.LogInformation(
                    "Respuesta exitosa de {Action} con código 200",
                    actionName);
            }
             
            if (context.Result is ObjectResult errorResult &&
                errorResult.StatusCode >= 400)
            {
                _logger.LogWarning(
                    "Respuesta con error {StatusCode} en {Action}",
                    errorResult.StatusCode,
                    actionName);
            }
        }
    }

}

