using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;

//parametros de configuracao do app
var builder = WebApplication.CreateBuilder(args);

//configuracao com o banco
builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"])    
);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/rangos/{numero}/{nome}", (int numero, string nome) => {
    return nome + "cc23 " + numero;
});


app.MapGet("/rango/{nome}", (RangoDbContext rangoDbContext, string nome) => {
    return rangoDbContext.Rangos.FirstOrDefault(x => x.Nome == nome);
});

//tras apenas 1 dado baseado no id enviado como parametro. Contexto, parametro. Quando a rota tiver o mesmo nome e passar 1 parametro, importante definir o tipo do parametro.
app.MapGet("/rango/{id:int}", (RangoDbContext rangoDbContext, int id) => {
    return rangoDbContext.Rangos.FirstOrDefault(x => x.Id == id);
});

//usando parameter Binding, o mesmo que o de cima, mas com o FromQuery. Tem tambem [FromHeader(Name = "RangoId")]
app.MapGet("/rango", (RangoDbContext rangoDbContext, [FromHeader(Name = "RangoId")]int id) => {
    return rangoDbContext.Rangos.FirstOrDefault(x => x.Id == id);
});

//tras todos os dados da tabela Rangos
app.MapGet("/rangos", (RangoDbContext rangoDbContext) => {
    return rangoDbContext.Rangos;
});

app.Run();
