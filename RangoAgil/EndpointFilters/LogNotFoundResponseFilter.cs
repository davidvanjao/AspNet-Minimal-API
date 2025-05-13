
using Microsoft.Win32;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;

namespace RangoAgil.API.EndpointFilters;

//É uma classe que implementa IEndpointFilter, uma interface usada em filtros de minimal APIs
public class LogNotFoundResponseFilter(ILogger<LogNotFoundResponseFilter> logger) : IEndpointFilter {

    //A instância de logger recebida é atribuída a um campo readonly para ser usada dentro do método.
    public readonly ILogger<LogNotFoundResponseFilter> _logger = logger;    

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
        
        var result = await next(context);
        var actualResults = (result is INestedHttpResult result1) ? result1.Result : (IResult)result;

        //Verifica se o resultado tem um código de status 404 (Not Found).
        if (actualResults is IStatusCodeHttpResult { StatusCode: (int)HttpStatusCode.NotFound }) {

            //Registra no log o caminho da requisição que gerou o erro 404.
            _logger.LogInformation($"Resource {context.HttpContext.Request.Path} was not found.");
        }

        //O filtro não altera o resultado, apenas o inspeciona e registra log se for 404.
        return result;
    }
}

