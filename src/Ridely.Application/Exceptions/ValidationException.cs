using System.Text.Json;

namespace Soloride.Application.Exceptions;
public class ValidationException(IEnumerable<ValidationError> Errors): Exception(JsonSerializer.Serialize(Errors))
{
    //public IEnumerable<ValidationError> Errors => Errors;
}
