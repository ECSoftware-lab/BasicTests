using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestsFilters.Filters;

namespace TestsFilters.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestCacheController : ControllerBase
    {
        private readonly ILogger<TestCacheController> _logger;

        public TestCacheController(ILogger<TestCacheController> logger)
        {
            _logger = logger;
        }

        // Endpoint CON cache
        [HttpGet("with-cache")]
        [ServiceFilter(typeof(CacheResourceFilter))]
        public IActionResult GetWithCache()
        {
            _logger.LogInformation("Ejecutando action GetWithCache - esto solo debería verse en cache MISS");

            return Ok(new
            {
                message = "Response with cache",
                timestamp = DateTime.UtcNow,
                randomNumber = Random.Shared.Next(1000, 9999)
            });
        }

        // Endpoint SIN cache para comparar
        [HttpGet("without-cache")]
        public IActionResult GetWithoutCache()
        {
            _logger.LogInformation("Ejecutando action GetWithoutCache - esto se ve siempre");

            return Ok(new
            {
                message = "Response without cache",
                timestamp = DateTime.UtcNow,
                randomNumber = Random.Shared.Next(1000, 9999)
            });
        }

        // Endpoint con parámetros (para probar cache con query string)
        [HttpGet("with-cache-params")]
        [ServiceFilter(typeof(CacheResourceFilter))]
        public IActionResult GetWithCacheAndParams([FromQuery] string? name)
        {
            _logger.LogInformation("Ejecutando action con parámetro: {Name}", name ?? "sin nombre");

            return Ok(new
            {
                message = $"Hello {name ?? "Anonymous"}",
                timestamp = DateTime.UtcNow,
                randomNumber = Random.Shared.Next(1000, 9999)
            });
        }

        // POST no se cachea
        [HttpPost("test-post")]
        [ServiceFilter(typeof(CacheResourceFilter))]
        public IActionResult TestPost([FromBody] object data)
        {
            _logger.LogInformation("POST request - nunca se cachea");

            return Ok(new
            {
                message = "POST requests are never cached",
                timestamp = DateTime.UtcNow,
                receivedData = data
            });
        }
    }
}
