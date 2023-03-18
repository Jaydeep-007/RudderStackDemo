using Microsoft.EntityFrameworkCore;
using RudderStack;
using RudderStackDemo.Data;
using RudderStackDemo.Middlewares;
using RudderStackDemo.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddDbContext<DbContextClass>
(o => o.UseInMemoryDatabase("RudderStackDemo"));

//Initialize RudderAnalytics with Write Key of Source and Data plane URL.
RudderAnalytics.Initialize("<WRITE_KEY>", new RudderConfig(dataPlaneUrl: "<DATA_PLANE_URL>"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DbContextClass>();

    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestMiddleware>();
app.UseMiddleware<ResponseMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
