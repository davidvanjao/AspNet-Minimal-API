using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndpointHandlers;
public static class IngredientesHandlers {

    public static async Task<Results<NotFound, Ok<IEnumerable<IngredienteDTO>>>> GetIngredientesAsync
        (RangoDbContext rangoDbContext,
        IMapper mapper,
        int rangoId) {

        var rangoEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);
        if (rangoEntity == null) {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(mapper.Map<IEnumerable<IngredienteDTO>>((await rangoDbContext.Rangos
            .Include(rango => rango.Ingredientes) //faz o join entre as tabelas
            .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes));
    }
}

