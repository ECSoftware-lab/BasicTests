using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace TestsFilters.Filters
{
    public class PerformanceMonitorFilter : IResultFilter
    {
        private readonly ILogger<PerformanceMonitorFilter> _logger;
        private Stopwatch _stopwatch;

        public PerformanceMonitorFilter(ILogger<PerformanceMonitorFilter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public void OnResultExecuting(ResultExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
            var actionName = context.ActionDescriptor.DisplayName;
            _logger.LogInformation($"Inicio de ejecución del resultado para {actionName}", actionName);
        }
        public void OnResultExecuted(ResultExecutedContext context)
        {
            _stopwatch.Stop();
            var actionName = context.ActionDescriptor.DisplayName;
            var elapsedMs = _stopwatch.ElapsedMilliseconds;
            var statusCode = context.HttpContext.Response.StatusCode;
            _logger.LogInformation($"Resultado de {actionName} ejecutado en {elapsedMs} ms con estado {statusCode}", actionName, elapsedMs, statusCode);

        }

      
    }
}
