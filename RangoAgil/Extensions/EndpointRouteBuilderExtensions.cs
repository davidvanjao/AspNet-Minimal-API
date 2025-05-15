using Microsoft.AspNetCore.Identity;
using RangoAgil.API.EndpointFilters;
using RangoAgil.API.EndpointHandlers;

namespace RangoAgil.API.Extensions; 
public static class EndpointRouteBuilderExtensions {

    public static void RegisterRangosEndPoints(this IEndpointRouteBuilder endpointRouteBuilder) {

        endpointRouteBuilder.MapGroup("/identity").MapIdentityApi<IdentityUser>();//adiciona o identity com o banco de dados

        var rangosEndpoint = endpointRouteBuilder.MapGroup("/rangos")
            .RequireAuthorization(); //adiciona o middleware de autorizacao

        var rangosComIdEndpoint = rangosEndpoint.MapGroup("/{rangoId:int}");

        var rangodComIdAndLockedEndpoints = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}")
            .RequireAuthorization("RequireAdminFromBrazil")
            .RequireAuthorization()
            .AddEndpointFilter(new RangoIsLockedFilter(6)) //verifica o 6 depois o 5
            .AddEndpointFilter(new RangoIsLockedFilter(5));

        endpointRouteBuilder.MapGet("/pratos/{pratoid:int}", (int pratoid) => $"O prato {pratoid} é delicioso!")
            .WithOpenApi(operation => {
                operation.Deprecated = true; //marca o endpoint como obsoleto
                return operation;
            })
            .WithSummary("Endpoint sera excluido")
            .WithDescription("Endpoint de exemplo para mostrar que o endpoint sera excluido.");

        rangosEndpoint.MapGet("", RangosHandlers.GetRangosAsync)
            .WithOpenApi()
            .WithSummary("Tras todos os pratos")
            .WithDescription("Endpoint de exemplo para mostrar todos os pratos cadastrados");

        rangosComIdEndpoint.MapGet("", RangosHandlers.GetRangoByIdAsync).WithName("GetRangos")
            .AllowAnonymous(); //permite acesso anonimo ao endpoint

        rangosEndpoint.MapPost("", RangosHandlers.CreateRangoAsync)
            .AddEndpointFilter<ValidateAnnotationFilter>(); //adiciona o filtro de validacao de modelo

        rangodComIdAndLockedEndpoints.MapPut("", RangosHandlers.UpdateRangoAsync);

        rangodComIdAndLockedEndpoints.MapDelete("", RangosHandlers.DeleteRangoAsync)
            .AddEndpointFilter<LogNotFoundResponseFilter>();

    }

    public static void RegisterIngredientesEndPoints(this IEndpointRouteBuilder endpointRouteBuilder) {

        var ingredientesEndpoint = endpointRouteBuilder.MapGroup("/rangos/{rangoId:int}/ingredientes")
            .RequireAuthorization(); //adiciona o middleware de autorizacao

        ingredientesEndpoint.MapGet("", IngredientesHandlers.GetIngredientesAsync);
        ingredientesEndpoint.MapPost("", () => {
            throw new NotImplementedException();
        });  
    }


}
