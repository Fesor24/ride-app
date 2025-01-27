namespace Ridely.Shared.Helper.Keys;
public static class Cache
{
    public static class Login
    {
        public static string Key(string phoneNo, int appInstance) =>
            $"LOGIN-{phoneNo}-{appInstance}";
    }

    public static class Register
    {
        public static string Key(string phoneNo, int appInstance) =>
            $"REGISTER-{phoneNo}-{appInstance}";
    }

    public static class UserAuth
    {
        public static string Key(string phoneNo, int appInstance) =>
            $"USERAUTH-{phoneNo}-{appInstance}";
    }

    public static class BankAccount
    {
        public static string Key(string phoneNo) =>
            $"BANKACCOUNT-{phoneNo}";
    }

    public static class ProcessWithdrawal
    {
        public static string Key(string phoneNo) =>
            $"PROCESSWITHDRAW-{phoneNo}";
    }

    public static class Calls
    {
        public static string Key(string phoneNo) =>
            $"CALLS-{phoneNo}";
    }
}
