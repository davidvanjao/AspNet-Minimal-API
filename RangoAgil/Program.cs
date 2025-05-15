using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
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

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<RangoDbContext>();//adiciona o identity com o banco de dados

//Procura dentro das dlls o profile que tem o mapeamento entre as entidades e os DTOs
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); //adiciona o automapper para fazer o mapeamento entre as entidades e os DTOs

builder.Services.AddProblemDetails();

//adiciona o jwt bearer para autenticacao
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminFromBrazil", policy => {
        policy
        .RequireRole("admin")
        .RequireClaim("country", "Brazil");
    });


//adiciona o swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("TokenAuthRango",
        new() {
            Name = "Authorization",
            Description = "Token de autenticaçãoo e autorização",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            In = ParameterLocation.Header
        }
    );
    options.AddSecurityRequirement(new()
    {
        {
            new ()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "TokenAuthRango"
                }
            },
            new List<string>()
        }
    });
});

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

app.UseHttpsRedirection(); //redireciona para o https

app.UseAuthentication(); //adiciona o middleware de autenticacao
app.UseAuthorization(); //adiciona o middleware de autorizacao

if (app.Environment.IsDevelopment()) //verifica se o ambiente e de desenvolvimento
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.MapGet("/", () => "Hello World!");

app.RegisterRangosEndPoints(); //o parametro enviado e o proprio app
app.RegisterIngredientesEndPoints();

app.Run();
