using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndpointHandlers;

//manipulador de endpoints
public static class RangosHandlers {

    public static async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> GetRangosAsync
        (RangoDbContext rangoDbContext,
        IMapper mapper, //faz o mapeamento entre as entidades e os DTOs
        ILogger<RangoDTO> logger,
        [FromQuery(Name = "name")] string? rangoNome) {

        var rangosEntity = await rangoDbContext.Rangos
                                    .Where(x => rangoNome == null || x.Nome.ToLower().Contains(rangoNome.ToLower())) //se for null, traz todos os rangos, se nao, traz apenas os que tem o nome especificado. Converte texto para minusculo.
                                    .ToListAsync();
        if (rangosEntity.Count <= 0 || rangosEntity == null) {
            logger.LogInformation("Nenhum rango encontrado com o nome: {RangoNome}", rangoNome);
            return TypedResults.NoContent();
        } else {
            logger.LogInformation("Rangos encontrados: {RangosCount}", rangosEntity.Count);
            return TypedResults.Ok(mapper.Map<IEnumerable<RangoDTO>>(rangosEntity));
        }

    }

    public static async Task<Results<NotFound, Ok<RangoDTO>>> GetRangoByIdAsync
        (RangoDbContext rangoDbContext,
        IMapper mapper, //faz o mapeamento entre as entidades e os DTOs
        int rangoId) {

        var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);
        if (rangoEntity == null) {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(mapper.Map<RangoDTO>(
            await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId))
        );
    }

    public static async Task<CreatedAtRoute<RangoDTO>> CreateRangoAsync
        (RangoDbContext rangoDbContext,
        IMapper mapper,
        [FromBody] RangoParaCriacaoDTO rangoParaCriacaoDTO) {

        var rangoEntity = mapper.Map<Rango>(rangoParaCriacaoDTO);
        rangoDbContext.Add(rangoEntity);
        await rangoDbContext.SaveChangesAsync();
        var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);

        return TypedResults.CreatedAtRoute(
            rangoToReturn,
            "GetRangos", 
            new { rangoId = rangoToReturn.Id });

    }


    public static async Task<Results<NotFound, Ok>> UpdateRangoAsync
        (RangoDbContext rangoDbContext,
        IMapper mapper, //faz o mapeamento entre as entidades e os DTOs
        int rangoId, //É o id que veio na URL.
        [FromBody] RangoParaAtualizacaoDTO rangoParaAtualizacaoDTO) { //É o objeto enviado no corpo da requisição (Body) 

        var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);
        if (rangoEntity == null) {
            return TypedResults.NotFound();
        }
        mapper.Map(rangoParaAtualizacaoDTO, rangoEntity); //Atualiza os campos do rangoEntity com os dados do rangoParaAtualizacaoDTO
        await rangoDbContext.SaveChangesAsync();
        return TypedResults.Ok();

    }

    public static async Task<Results<NotFound, NoContent>> DeleteRangoAsync
        (RangoDbContext rangoDbContext,
        int rangoId) {

        var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);
        if (rangoEntity == null) {
            return TypedResults.NotFound();
        }
        rangoDbContext.Rangos.Remove(rangoEntity); //Remove o rango do banco de dados, mas ainda não salva as mudanças.
        await rangoDbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    }

}