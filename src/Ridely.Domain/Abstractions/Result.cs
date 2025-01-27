namespace Soloride.Domain.Abstractions;
public class Result
{
    public Result()
    {
        IsSuccessful = true;
        Error = Error.None;
    }

    public Result(Error error)
    {
        IsSuccessful = false;
        Error = error;
    }

    public bool IsSuccessful { get; private set; }
    public bool IsFailure => !IsSuccessful;
    public Error Error { get; private set; }

    public static Result Success() => new();
    public static Result Failure(Error error) => new(error);
}
