namespace Ridely.Domain.Abstractions;
public class Error
{
    public Error()
    {
        Code = string.Empty;
        Message = string.Empty;
    }

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public static Error None = new();
    public string Code { get; private set; }
    public string Message { get; private set; }
    public ICollection<ValidationError> ValidationErrors { get; set; } = [];

    public static NotFound NotFound(string code, string message) => new(code, message);
    public static BadRequest BadRequest(string code, string message) => new(code, message);
    public static Unauthorized Unauthorized(string code, string message) => new(code, message);

}

public class NotFound(string code, string message) : Error(code, message)
{

}

public class BadRequest(string code, string message) : Error(code, message)
{

}

public class Unauthorized(string code, string message) : Error(code, message)
{

}

public sealed record ValidationError(string Property, string Message);
