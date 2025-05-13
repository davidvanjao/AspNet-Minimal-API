
namespace RangoAgil.API.EndpointFilters;
public class RangoIsLockedFilter : IEndpointFilter {

    public readonly int _lockedRangoId;

    public RangoIsLockedFilter(int lockedRangoId) {
        _lockedRangoId = lockedRangoId;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {

        int rangoId;

        if(context.HttpContext.Request.Method == "PUT") { //verifica se o metodo e PUT
            rangoId = context.GetArgument<int>(2); //numero se refere a posicao do argumento na lista de argumentos
        } else if(context.HttpContext.Request.Method == "DELETE") { //verifica se o metodo e DELETE
            rangoId = context.GetArgument<int>(1); //numero se refere a posicao do argumento na lista de argumentos
        } else {
            throw new NotSupportedException("Método não suportado");
        }

        if (rangoId == _lockedRangoId) {
            return TypedResults.Problem(new() {
                Status = 400,
                Title = "Rango não pode ser atualizado ou deletado",
                Detail = "Rango não pode ser atualizado ou deletado porque é o rango do tropeiro"
            });
        }
        var result = await next.Invoke(context); //chama o proximo passo que no caso e o UpdateRangoAsync
        return result;
        
    }
}

