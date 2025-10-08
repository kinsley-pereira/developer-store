using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Sales.Api.Middlewares;
using Sales.Application.Interfaces;
using Sales.Application.Mapping;
using Sales.Application.Sales.Queries.GetSales;
using Sales.Application.Validators;
using Sales.Infra.Data;
using Sales.Infra.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<CreateSaleCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateSaleCommandValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Sales API", Version = "v1" }));

builder.Services.AddDbContext<IAppDbContext, AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddMaps(typeof(SalesProfile).Assembly);
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetSalesQuery).Assembly);
});


var mongoConn = builder.Configuration.GetConnectionString("Mongo");
if (!string.IsNullOrWhiteSpace(mongoConn))
{
    builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConn));
    builder.Services.AddSingleton<IEventPublisher>(sp =>
        new MongoEventPublisher(sp.GetRequiredService<IMongoClient>(), builder.Configuration.GetValue<string>("MongoDbName") ?? "devstore", sp.GetRequiredService<ILogger<MongoEventPublisher>>()));
}
else
{
    builder.Services.AddSingleton<IEventPublisher, NullEventPublisher>();
}

builder.Services.AddLogging();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.MapControllers();
app.UseGlobalExceptionHandler();

app.Run();
