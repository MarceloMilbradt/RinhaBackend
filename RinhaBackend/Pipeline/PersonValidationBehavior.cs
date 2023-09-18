using MediatR;
using RinhaBackend.Cache;
using RinhaBackend.Persons;
using RinhaBackend.Persons.Commands;

namespace RinhaBackend.Pipeline;

internal class PersonValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICreatePersonRequestWithValidation
    where TResponse : CreatePersonResult
{
    private readonly IRedisCacheSevice _redisCacheService;

    public PersonValidationBehavior(IRedisCacheSevice redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Apelido))
            return (TResponse)CreatePersonResult.Fail();

        if (string.IsNullOrWhiteSpace(request.Nome))
            return (TResponse)CreatePersonResult.Fail();

        if (request.Stack != null && request.Stack.Any(s => string.IsNullOrWhiteSpace(s)))
            return (TResponse)CreatePersonResult.Fail();

        var hasAnyWithApelido = await _redisCacheService.KeyExistsAsync(request.Apelido);
        if (hasAnyWithApelido)
            return (TResponse)CreatePersonResult.Fail();

        return await next();
    }
}
