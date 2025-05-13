using RangoAgil.API.EndpointFilters;
using RangoAgil.API.EndpointHandlers;

namespace RangoAgil.API.Extensions; 
public static class EndpointRouteBuilderExtensions {

    public static void RegisterRangosEndPoints(this IEndpointRouteBuilder endpointRouteBuilder) {

        var rangosEndpoint = endpointRouteBuilder.MapGroup("/rangos");
        var rangosComIdEndpoint = rangosEndpoint.MapGroup("/{rangoId:int}");

        var rangodComIdAndLockedEndpoints = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}")
            .AddEndpointFilter(new RangoIsLockedFilter(6)) //verifica o 6 depois o 5
            .AddEndpointFilter(new RangoIsLockedFilter(5));

        rangosEndpoint.MapGet("", RangosHandlers.GetRangosAsync);

        rangosComIdEndpoint.MapGet("", RangosHandlers.GetRangoByIdAsync).WithName("GetRangos");

        rangosEndpoint.MapPost("", RangosHandlers.CreateRangoAsync)
            .AddEndpointFilter<ValidateAnnotationFilter>(); //adiciona o filtro de validacao de modelo

        //implementando um filtro
        //rangosComIdEndpoint.MapPut("", RangosHandlers.UpdateRangoAsync)
        //    .AddEndpointFilter(new RangoIsLockedFilter(6))
        //    .AddEndpointFilter(new RangoIsLockedFilter(5));

        //rangosComIdEndpoint.MapDelete("", RangosHandlers.DeleteRangoAsync)
        //    .AddEndpointFilter(new RangoIsLockedFilter(6))
        //    .AddEndpointFilter(new RangoIsLockedFilter(5));

        rangodComIdAndLockedEndpoints.MapPut("", RangosHandlers.UpdateRangoAsync);

        rangodComIdAndLockedEndpoints.MapDelete("", RangosHandlers.DeleteRangoAsync)
            .AddEndpointFilter<LogNotFoundResponseFilter>();

    }

    public static void RegisterIngredientesEndPoints(this IEndpointRouteBuilder endpointRouteBuilder) {

        var ingredientesEndpoint = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}/ingredientes");

        ingredientesEndpoint.MapGet("", IngredientesHandlers.GetIngredientesAsync);
        ingredientesEndpoint.MapPost("", () => {
            throw new NotImplementedException();
        });  
    }


}
