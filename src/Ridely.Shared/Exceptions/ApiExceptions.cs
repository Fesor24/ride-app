namespace Ridely.Shared.Exceptions;
public class ApiUnauthorizedException(string message) : Exception(message)
{
}

public class ApiValidationException(string message) : Exception(message) { }

public class ApiNotFoundException(string message) : Exception(message) { }

public class ApiBadRequestException(string message) : Exception(message) { }

public class NotFoundException(string message) : Exception(message) { }
