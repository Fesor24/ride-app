using Ridely.Domain.Abstractions;

namespace RidelyAPI.Shared;
public class ApiResponse
{
    public ApiResponse()
    {
        Ok = true;
        Error = Error.None;
    }

    public ApiResponse(Error error)
    {
        Error = error;
        Ok = false;
    }

    public bool Ok { get; init; }

    public Error Error { get; init; }
}

public class ApiResponse<TData>(TData data) : ApiResponse()
{
    public TData Data => data;
}
