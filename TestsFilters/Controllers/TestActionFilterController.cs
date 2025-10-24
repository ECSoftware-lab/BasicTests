using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestsFilters.Dtos;
using TestsFilters.Filters;

namespace TestsFilters.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ValidationActionFilter))]
    public class TestActionFilterController : ControllerBase
    {
        private readonly ILogger<TestActionFilterController> _logger;

        public TestActionFilterController(ILogger<TestActionFilterController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Endpoint simple que retorna datos - verás el wrapper con metadata
        /// </summary>
        [HttpGet("simple")]
        public IActionResult GetSimple()
        {
            return Ok(new { message = "Hello World", value = 42 });
        }

        /// <summary>
        /// Endpoint con validación - envía datos inválidos para ver el manejo de errores
        /// </summary>
        [HttpPost("create-user")]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            // Si llegamos aquí, las validaciones pasaron
            _logger.LogInformation("Usuario válido recibido: {Name}", request.Name);

            return Ok(new
            {
                id = Guid.NewGuid(),
                name = request.Name,
                email = request.Email,
                age = request.Age,
                createdAt = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Endpoint con parámetros opcionales - prueba enviando null
        /// </summary>
        [HttpPut("update-user/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            return Ok(new
            {
                id,
                message = "Usuario actualizado",
                changes = request
            });
        }

        /// <summary>
        /// Endpoint con query strings y validaciones
        /// </summary>
        [HttpGet("search")]
        public IActionResult Search([FromQuery] SearchRequest request)
        {
            // Simular búsqueda
            var results = Enumerable.Range(1, request.Limit)
                .Select(i => new
                {
                    id = i,
                    name = $"Result {i} for '{request.Query}'",
                    score = Random.Shared.NextDouble()
                })
                .ToList();

            return Ok(results);
        }

        /// <summary>
        /// Endpoint lento para probar el tracking de tiempo
        /// </summary>
        [HttpGet("slow")]
        public async Task<IActionResult> SlowEndpoint([FromQuery] int delayMs = 2000)
        {
            await Task.Delay(delayMs);
            return Ok(new { message = "This was slow", delayMs });
        }

        /// <summary>
        /// Endpoint que lanza una excepción - verás el manejo automático
        /// </summary>
        [HttpGet("throw-error")]
        public IActionResult ThrowError()
        {
            throw new InvalidOperationException("Este es un error intencional para testing");
        }

        /// <summary>
        /// Endpoint con múltiples parámetros
        /// </summary>
        [HttpPost("complex")]
        public IActionResult ComplexEndpoint(
            [FromQuery] string category,
            [FromQuery] int? priority,
            [FromBody] CreateUserRequest user)
        {
            return Ok(new
            {
                category,
                priority,
                user,
                processed = true
            });
        }

        /// <summary>
        /// Endpoint que retorna un error 404 manualmente
        /// </summary>
        [HttpGet("not-found/{id}")]
        public IActionResult GetById(int id)
        {
            // Simular que no se encontró
            if (id > 100)
            {
                return NotFound(new { message = $"Recurso con ID {id} no encontrado" });
            }

            return Ok(new { id, name = $"Resource {id}" });
        }
    }
}
