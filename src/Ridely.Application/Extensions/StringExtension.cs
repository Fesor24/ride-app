namespace Soloride.Application.Extensions;
internal static class StringExtension
{
    internal static string ToPhoneNumber(this string phoneNumber)
    {
        if(string.IsNullOrEmpty(phoneNumber)) return string.Empty;

        string number = phoneNumber.Replace(" ", "");

        if (!number.StartsWith("0"))
        {
            if (number.StartsWith("+234"))
                number = number.Replace("+234", "0");

            else
                number = "0" + number;
        }

        return number;
    }
}
