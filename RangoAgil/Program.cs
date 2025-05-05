using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.EndpointHandlers;
using RangoAgil.API.Extensions;
using System.Net;

//parametros de configuracao do app
var builder = WebApplication.CreateBuilder(args);

//configuracao com o banco
builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"])    
);

//Procura dentro das dlls o profile que tem o mapeamento entre as entidades e os DTOs
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); //adiciona o automapper para fazer o mapeamento entre as entidades e os DTOs

builder.Services.AddProblemDetails();

var app = builder.Build();

if(!app.Environment.IsDevelopment()) //verifica se o ambiente e de desenvolvimento
{
    app.UseExceptionHandler();

    //adiciona o middleware de tratamento de excecao. Referencias que pode ser usado.
    //app.UseExceptionHandler(configureAplicationBuider => {

    //    configureAplicationBuider.Run(
    //        async context => {
    //            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    //            context.Response.ContentType = "text/html";
    //            await context.Response.WriteAsync("<h1>Ocorreu um erro inesperado</h1>");
    //        });

    //}); 
}



app.MapGet("/", () => "Hello World!");

app.RegisterRangosEndPoints(); //o parametro enviado e o proprio app
app.RegisterIngredientesEndPoints();

app.Run();
