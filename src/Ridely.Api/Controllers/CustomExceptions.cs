namespace SolorideAPI.Controllers;

public sealed class ApiUnauthorizedException(string message) : Exception(message);
