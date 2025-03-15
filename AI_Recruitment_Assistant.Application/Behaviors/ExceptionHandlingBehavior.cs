using AI_Recruitment_Assistant.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AI_Recruitment_Assistant.Application.Behaviors;

public sealed class ExceptionHandlingBehavior<TRequest, TResponse>(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request failed: {RequestType}", typeof(TRequest).Name);

            Error error = ex switch
            {
                KeyNotFoundException => Error.NotFoundInstance,
                ArgumentException => Error.ValidationFailure,
                _ => Error.Unexpected
            };

            return (TResponse)Result.Failure(error);
        }
    }
}
