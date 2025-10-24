using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TestsFilters.Filters
{
    public class FakeTockenAuthoriztionFilter : IAuthorizationFilter
    {
        private const string ExpectedToken = "Bearer Fake-Token-123";
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != ExpectedToken)
            {
                context.HttpContext.Response.StatusCode = 401; // Unauthorized
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.WriteAsync("{\"message\":\"Unauthorized: Invalid or missing token.\"}");
            }
        }
    }
    public class FakeTockenAuthoriztionFilterAsync : IAsyncAuthorizationFilter
    {
        private const string ExpectedToken = "Bearer Fake-Token-123";
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != ExpectedToken)
            {
                context.HttpContext.Response.StatusCode = 401;
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsync("{\"message\":\"Unauthorized: Invalid or missing token async.\"}");
                context.Result = new EmptyResult(); // Evita que continúe la ejecución
            }
        }
    }
}
