namespace Soloride.Application.Abstractions.Security;
public interface IHashService
{
    string HashAndSaltValue(string value);
    string HashAndSaltValue(string value, string salt);
    string HashValue(string value);
    bool ComparePassword(string password, string value);
    string GenerateSalt();
}
