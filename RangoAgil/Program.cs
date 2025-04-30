using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.EndpointHandlers;
using RangoAgil.API.Extensions;

//parametros de configuracao do app
var builder = WebApplication.CreateBuilder(args);

//configuracao com o banco
builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"])    
);

//Procura dentro das dlls o profile que tem o mapeamento entre as entidades e os DTOs
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); //adiciona o automapper para fazer o mapeamento entre as entidades e os DTOs

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.RegisterRangosEndPoints(); //o parametro enviado e o proprio app
app.RegisterIngredientesEndPoints();

app.Run();
