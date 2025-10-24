using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace TestsFilters.Filters
{
    public class CacheResourceFilter : IResourceFilter
    {
        private readonly IMemoryCache _cache;
        private readonly int _cacheDurationInSeconds;
        private readonly ILogger<CacheResourceFilter> _logger;

        public CacheResourceFilter(
         IMemoryCache cache,
         ILogger<CacheResourceFilter> logger,
         int cacheDurationInSeconds = 300)
        {
            _cache = cache;
            _logger = logger;
            _cacheDurationInSeconds = cacheDurationInSeconds;
        }
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context.HttpContext.Request.Method != HttpMethods.Get)
            {
                return;
            }
            var cacheKey = GenerateCacheKey(context.HttpContext.Request);
            if (_cache.TryGetValue(cacheKey, out ObjectResult? cachedResult))
            {
                _logger.LogInformation("Cache HIT para {CacheKey}", cacheKey);

                // Si existe en cache, establecer el resultado y cortocircuitar el pipeline
                context.Result = cachedResult;
                context.HttpContext.Response.Headers["X-Cache"] = "HIT";
                return;
            }

            _logger.LogInformation("Cache MISS para {CacheKey}", cacheKey);
            context.HttpContext.Response.Headers["X-Cache"] = "MISS";

        }
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            if (context.HttpContext.Request.Method != HttpMethods.Get)
            {
                return;
            }
            if (context.Result is ObjectResult objectResult &&
            objectResult.StatusCode == 200)
            {
                var cacheKey = GenerateCacheKey(context.HttpContext.Request);

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_cacheDurationInSeconds),
                    SlidingExpiration = TimeSpan.FromSeconds(_cacheDurationInSeconds / 2)
                };

                _cache.Set(cacheKey, objectResult, cacheOptions);

                _logger.LogInformation(
                    "Respuesta cacheada para {CacheKey} por {Duration} segundos",
                    cacheKey,
                    _cacheDurationInSeconds);
            }
        }
        private string GenerateCacheKey(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append($"{request.Path}");

            if (request.QueryString.HasValue)
            {
                keyBuilder.Append(request.QueryString.Value);
            }

            return keyBuilder.ToString().ToLowerInvariant();
        }

    }
}
