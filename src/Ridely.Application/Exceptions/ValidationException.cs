using System.Text.Json;

namespace Ridely.Application.Exceptions;
public class ValidationException(IEnumerable<ValidationError> Errors): Exception(JsonSerializer.Serialize(Errors))
{
    //public IEnumerable<ValidationError> Errors => Errors;
}
