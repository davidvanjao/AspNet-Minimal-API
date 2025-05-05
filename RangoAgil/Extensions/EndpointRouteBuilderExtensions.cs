using RangoAgil.API.EndpointHandlers;

namespace RangoAgil.API.Extensions; 
public static class EndpointRouteBuilderExtensions {

    public static void RegisterRangosEndPoints(this IEndpointRouteBuilder endpointRouteBuilder) {

        var rangosEndpoint = endpointRouteBuilder.MapGroup("/rangos");
        var rangosComIdEndpoint = rangosEndpoint.MapGroup("/{rangoId:int}");

        rangosEndpoint.MapGet("", RangosHandlers.GetRangosAsync);

        rangosComIdEndpoint.MapGet("", RangosHandlers.GetRangoByIdAsync).WithName("GetRangos");

        rangosEndpoint.MapPost("", RangosHandlers.CreateRangoAsync);

        rangosComIdEndpoint.MapPut("", RangosHandlers.UpdateRangoAsync);

        rangosComIdEndpoint.MapDelete("", RangosHandlers.DeleteRangoAsync);

    }

    public static void RegisterIngredientesEndPoints(this IEndpointRouteBuilder endpointRouteBuilder) {

        var ingredientesEndpoint = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}/ingredientes");

        ingredientesEndpoint.MapGet("", IngredientesHandlers.GetIngredientesAsync);
        ingredientesEndpoint.MapPost("", () => {
            throw new NotImplementedException();
        });  
    }


}
