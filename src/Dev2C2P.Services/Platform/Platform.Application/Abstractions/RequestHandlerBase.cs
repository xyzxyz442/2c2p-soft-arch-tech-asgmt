using Dev2C2P.Services.Platform.Common;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dev2C2P.Services.Platform.Application.Abstractions;

public abstract class RequestHandlerBase<TRequest, TResponseResult>
    : IRequestHandler<TRequest, ErrorOr<Result<TResponseResult>>>
    where TRequest : IRequest<ErrorOr<Result<TResponseResult>>>
{
    protected readonly ILogger<RequestHandlerBase<TRequest, TResponseResult>> Logger;

    protected RequestHandlerBase(ILogger<RequestHandlerBase<TRequest, TResponseResult>> logger)
    {
        Logger = logger;
    }

    public async Task<ErrorOr<Result<TResponseResult>>> Handle(
        TRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogDebug("{LogPrefix}: handle {Type}.", GetLogPrefix(), typeof(TRequest).Name);

        try
        {
            return await DoHandle(request, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "{LogPrefix}: handle {Type}.", GetLogPrefix(), typeof(TRequest).Name);


            return Error.Failure("MediatorHandleException", GetDefautErrorMessage());
        }
    }

    protected virtual string GetDefautErrorMessage() => "An error occurred while processing the request.";

    protected abstract string GetLogPrefix();

    protected abstract Task<ErrorOr<Result<TResponseResult>>> DoHandle(
        TRequest request,
        CancellationToken cancellationToken);
}
