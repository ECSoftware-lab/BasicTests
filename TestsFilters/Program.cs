using TestsFilters.Filters;
using DbContexFake;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCustomDbContext();
builder.Services.AddControllers();

builder.Services.AddScoped<FakeTockenAuthoriztionFilter>();
builder.Services.AddScoped<FakeTockenAuthoriztionFilterAsync>();

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
