using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

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

app.MapGet("/rangos/{numero}/{nome}", (int numero, string nome) => { //isso é um endpoint
    return nome + "cc23 " + numero;
});

//return um task do tipo Results<NoContent, Ok<List<Rango>>> com documentacao
app.MapGet("/rangos", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>>
    (RangoDbContext rangoDbContext,
    IMapper mapper, //faz o mapeamento entre as entidades e os DTOs
    [FromQuery(Name = "name")]string? rangoNome) => {

    var rangosEntity = await rangoDbContext.Rangos
                                .Where(x => rangoNome == null || x.Nome.ToLower().Contains(rangoNome.ToLower())) //se for null, traz todos os rangos, se nao, traz apenas os que tem o nome especificado. Converte texto para minusculo.
                                .ToListAsync();

    if(rangosEntity.Count <= 0 || rangosEntity == null) {
        return TypedResults.NoContent();
    } else {
        return TypedResults.Ok(mapper.Map<IEnumerable<RangoDTO>>(rangosEntity));
    }
});

//endpoint HTTP GET que retorna os ingredientes de um "rango"
app.MapGet("/rango/{rangoId:int}/ingredientes", async (
    RangoDbContext rangoDbContext,
    IMapper mapper, //faz o mapeamento entre as entidades e os DTOs
    int rangoId) => {

    return mapper.Map<IEnumerable<IngredienteDTO>>((await rangoDbContext.Rangos
                                .Include(rango => rango.Ingredientes) //faz o join entre as tabelas
                                .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes);
});

//metodo async utilizado para aguardar o resultado da consulta no banco de dados
app.MapGet("/rango/{nome}", async (RangoDbContext rangoDbContext, string nome) => {
    return await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Nome == nome);
});

//sem documentacao
//tras apenas 1 dado baseado no id enviado como parametro. Contexto, parametro. Quando a rota tiver o mesmo nome e passar 1 parametro, importante definir o tipo do parametro.
app.MapGet("/rango/{id:int}", async (
    RangoDbContext rangoDbContext,
    IMapper mapper, //faz o mapeamento entre as entidades e os DTOs
    int id) => {

    return mapper.Map<RangoDTO>(await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == id));
});

//usando parameter Binding, o mesmo que o de cima, mas com o FromQuery. Tem tambem [FromHeader(Name = "RangoId")]
app.MapGet("/rango", async (RangoDbContext rangoDbContext, [FromHeader(Name = "RangoId")]int id) => {
    return await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == id);
});

app.Run();
