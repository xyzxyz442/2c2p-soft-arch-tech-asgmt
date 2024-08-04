using Dev2C2P.Services.Platform.Common;
using ErrorOr;
using MediatR;

namespace Dev2C2P.Services.Platform.Application.Abstractions;

public abstract record RequestBase<T>
    : IRequest<ErrorOr<Result<T>>>
{
    protected RequestBase() { }
}
