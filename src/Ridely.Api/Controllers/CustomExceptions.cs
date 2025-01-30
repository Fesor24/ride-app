namespace Ridely.Api.Controllers;

public sealed class ApiUnauthorizedException(string message) : Exception(message);
