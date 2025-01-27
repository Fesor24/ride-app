namespace Ridely.Domain.Abstractions;
public class Result<TValue> : Result
{
    private readonly TValue _value;

    public Result(TValue value) : base()
    {
        _value = value;
    }

    public Result(Error error) : base(error) { }

    public TValue Value => IsSuccessful ?
        _value : throw new ArgumentNullException(nameof(Value));

    public static implicit operator Result<TValue>(TValue value) => new(value);
    public static implicit operator Result<TValue>(Error error) => new(error);

    public TResult Match<TResult>(Func<TValue, TResult> success, Func<Error, TResult> failure) =>
        IsSuccessful ? success(_value) : failure(Error);
}
