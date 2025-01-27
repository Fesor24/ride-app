namespace Soloride.Application.Extensions;
internal static class DateTimeExtension
{
    internal static string ToCustomDateString(this DateTime date) =>
        date.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
}
