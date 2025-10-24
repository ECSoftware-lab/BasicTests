using DbContexFake;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TestsFilters.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCustomDbContext();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<FakeTockenAuthoriztionFilter>();
builder.Services.AddScoped<FakeTockenAuthoriztionFilterAsync>();

// Registrar el filter como servicio con scoped lifetime
builder.Services.AddScoped<CacheResourceFilter>(provider =>
{
    var cache = provider.GetRequiredService<IMemoryCache>();
    var logger = provider.GetRequiredService<ILogger<CacheResourceFilter>>();
    return new CacheResourceFilter(cache, logger, cacheDurationInSeconds: 160); // 1 minuto para testing
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // personalizado maneje los errores de modelo, tenés que desactivar el filtro automático de[ApiController]

    options.SuppressModelStateInvalidFilter = true;
});

//builder.Services.AddControllersWithViews(options =>
//{
//    options.Filters.AddService<PerformanceMonitorFilter>();
//});

builder.Services.AddScoped<PerformanceMonitorFilter>(); 

builder.Services.AddScoped<ValidationActionFilter>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
