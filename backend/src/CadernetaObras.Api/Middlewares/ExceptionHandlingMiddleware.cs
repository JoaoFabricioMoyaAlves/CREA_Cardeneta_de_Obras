using System.Net;
using System.Text.Json;
using CadernetaObras.Application.Common;
using CadernetaObras.Domain.Exceptions;

namespace CadernetaObras.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var (status, mensagem) = Mapear(ex);

            if (status == HttpStatusCode.InternalServerError)
                _logger.LogError(ex, "Erro não tratado na API");

            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { erro = mensagem }));
        }
    }

    private static (HttpStatusCode, string) Mapear(Exception ex) => ex switch
    {
        UnauthorizedAppException => (HttpStatusCode.Unauthorized, ex.Message),
        ForbiddenException => (HttpStatusCode.Forbidden, ex.Message),
        NotFoundException => (HttpStatusCode.NotFound, ex.Message),
        ValidationAppException => (HttpStatusCode.BadRequest, ex.Message),
        DomainException => (HttpStatusCode.Conflict, ex.Message), // imutabilidade, assinatura duplicada
        _ => (HttpStatusCode.InternalServerError, "Ocorreu um erro inesperado. Tente novamente."),
    };
}
