using System;
using System.Globalization;

namespace SFA.DAS.ApprenticeApp.Pwa.Helpers
{
    public static class DateTimeExtensions
    {
        public static string ToIsoDate(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}
