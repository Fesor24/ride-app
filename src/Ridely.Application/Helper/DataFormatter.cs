namespace Ridely.Application.Helper;
public static class DataFormatter
{
    public static string FormatPhoneNo(string phoneNo)
    {
        string number = phoneNo.Replace(" ", "");

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
