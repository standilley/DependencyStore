using DependencyStore;
using DependencyStore.Repositories;
using DependencyStore.Repositories.Contracts;
using DependencyStore.Services;
using DependencyStore.Services.Contracts;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Configuration>();

builder.Services.AddScoped(x => new SqlConnection("CONN_STRING"));
builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
builder.Services.AddTransient<IDeliveryFeeService, DeliveryFeeService>();
builder.Services.AddTransient<IPromoCodeRepository, PromoCodeRepository>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();