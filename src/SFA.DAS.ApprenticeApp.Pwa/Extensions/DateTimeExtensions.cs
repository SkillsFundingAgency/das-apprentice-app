namespace SFA.DAS.ApprenticeApp.Web.Extensions;

public static class DateTimeExtensions
{
    public static string ToGdsFormat(this DateTime date)
    {
        return date.ToString("d MMMM yyyy");
    }
}