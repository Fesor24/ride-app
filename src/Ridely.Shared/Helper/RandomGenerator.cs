using HashidsNet;

namespace Ridely.Shared.Helper;
public static class RandomGenerator
{
    public static string GenerateCode(int codeLength)
    {
        var random = new Random();

        string code = string.Empty;

        for (int i = 0; i < codeLength; i++)
        {
            code += random.Next(0, 10).ToString();
        }

        return code;
    }

    public static string GenerateString(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        char[] charArr = new char[length];

        for (int i = 0; i < length; i++)
        {
            charArr[i] = chars[new Random().Next(chars.Length)];
        }

        return new string(charArr);
    }

    //public static string GenerateTrxNumber()
    //{
    //    var randomNumber = new Random().Next(1000, 10000);

    //    var uniqueNumber = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

    //    return $"{randomNumber}{uniqueNumber.Substring(2)}";
    //}

    public static string GenerateReferralCode(long userId)
    {
        var hashIds = new Hashids(minHashLength: 3);

        return hashIds.EncodeLong(userId);
    }
}
