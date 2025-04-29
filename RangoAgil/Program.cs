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

var rangosEndpoint = app.MapGroup("/rangos"); //cria um grupo de endpoints que começam com /rangos. Isso é útil para organizar os endpoints e aplicar middleware ou configurações específicas a um grupo de rotas.
var rangosComIdEndpoint = rangosEndpoint.MapGroup("/{rangoId:int}");
var ingredientesEndpoint = rangosComIdEndpoint.MapGroup("/ingredientes");

app.MapGet("/", () => "Hello World!");

//return um task do tipo Results<NoContent, Ok<List<Rango>>> com documentacao
rangosEndpoint.MapGet("", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>>
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
ingredientesEndpoint.MapGet("", async (
    RangoDbContext rangoDbContext,
    IMapper mapper, //faz o mapeamento entre as entidades e os DTOs
    int rangoId) => {

    return mapper.Map<IEnumerable<IngredienteDTO>>((await rangoDbContext.Rangos
                                .Include(rango => rango.Ingredientes) //faz o join entre as tabelas
                                .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes);
});

//sem documentacao
//tras apenas 1 dado baseado no id enviado como parametro. Contexto, parametro. Quando a rota tiver o mesmo nome e passar 1 parametro, importante definir o tipo do parametro.
rangosComIdEndpoint.MapGet("", async (
    RangoDbContext rangoDbContext,
    IMapper mapper, //faz o mapeamento entre as entidades e os DTOs
    int rangoId) => {

    return mapper.Map<RangoDTO>(await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId));

}).WithName("GetRangos"); //atribuindo um nome para o endpoint, assim fica mais facil de referenciar ele depois. O nome é o mesmo que o do metodo, mas com a primeira letra minuscula.

rangosEndpoint.MapPost("", async Task<CreatedAtRoute<RangoDTO>>(
    RangoDbContext rangoDbContext,
    IMapper mapper,
    [FromBody] RangoParaCriacaoDTO rangoParaCriacaoDTO //é o objeto que o cliente mandou no corpo da requisição (com os dados do rango que ele quer criar).é o objeto que o cliente mandou no corpo da requisição (com os dados do rango que ele quer criar).
    ) => {

    var rangoEntity = mapper.Map<Rango>(rangoParaCriacaoDTO); //Pega o objeto enviado (rangoParaCriacaoDTO) e transforma em um objeto do tipo Rango, que é a entidade do banco de dados.

    rangoDbContext.Add(rangoEntity); //Prepara o objeto para ser salvo no banco (mas ainda não salvou).

    await rangoDbContext.SaveChangesAsync(); //Agora sim, grava no banco.

    var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity); //transforma o que foi salvo num RangoDTO para mandar de volta para quem fez o POST.

    return TypedResults.CreatedAtRoute(
        rangoToReturn, 
        "GetRangos", 
        new { rangoId = rangoToReturn.Id }
    );

});

rangosComIdEndpoint.MapPut("", async Task<Results<NotFound, Ok>>(
    RangoDbContext rangoDbContext,
    IMapper mapper, //faz o mapeamento entre as entidades e os DTOs
    int rangoId, //É o id que veio na URL.
    [FromBody] RangoParaAtualizacaoDTO rangoParaAtualizacaoDTO) => { //É o objeto enviado no corpo da requisição (Body) 

    var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

    if (rangoEntity == null) {
        return TypedResults.NotFound();
    }
        
    mapper.Map(rangoParaAtualizacaoDTO, rangoEntity); //Atualiza os campos do rangoEntity com os dados do rangoParaAtualizacaoDTO
    await rangoDbContext.SaveChangesAsync();
    return TypedResults.Ok();

});

rangosComIdEndpoint.MapDelete("", async Task<Results<NotFound, NoContent>> (
    RangoDbContext rangoDbContext,
    int rangoId) => {

    var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

    if (rangoEntity == null) {
        return TypedResults.NotFound();
    }

    rangoDbContext.Rangos.Remove(rangoEntity); //Remove o rango do banco de dados, mas ainda não salva as mudanças.
    await rangoDbContext.SaveChangesAsync();
    return TypedResults.NoContent();

});

app.Run();
