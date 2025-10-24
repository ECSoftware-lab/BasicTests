using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestsFilters.Filters;

namespace TestsFilters.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestFiltersController : ControllerBase
    {
        [HttpGet("public")]
        [ServiceFilter(typeof(PerformanceMonitorFilter))]
        public IActionResult PublicEndpoint()
        {
            var t = "SD";
            return Ok(new { message = "This is a public endpoint." });
        }
        [HttpGet("protectedAuthoriztion")]
        [ServiceFilter(typeof(FakeTockenAuthoriztionFilter))]
        public IActionResult ProtectedEndpoint()
        {
            return Ok(new { message = "This is a protected endpoint." });
        }
        [HttpGet("protectedAuthoriztionAsync")]
        [ServiceFilter(typeof(FakeTockenAuthoriztionFilterAsync))]
        public IActionResult ProtectedEndpointAsync()
        {
            return Ok(new { message = "This is a protected endpoint. Async" });
        }
    }
}
