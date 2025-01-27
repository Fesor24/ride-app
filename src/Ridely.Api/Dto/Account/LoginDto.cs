using Soloride.Application.Features.Accounts;

namespace SolorideAPI.Dto.Account;

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class InitiateLoginRequest
{
    public string PhoneNo { get; set; }
    public ApplicationInstance AppInstance { get; set; }
}


