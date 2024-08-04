namespace Dev2C2P.Services.Platform.Common;

public record Result<T>
{
    public T? Data { get; }

    public Result(T? data) => Data = data;
}
