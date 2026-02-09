using System;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class DateModel
    {
        public DateModel() { }

        public DateModel(DateTime date) : this(date.Year, date.Month, date.Day) { }

        public DateModel(int year, int month, int day) =>
            (Year, Month, Day) = (year, month, day);

        public virtual int? Day { get; set; }
        public virtual int? Month { get; set; }
        public virtual int? Year { get; set; }

        // Return nullable DateTime so callers can distinguish "invalid/no date"
        public DateTime? Date => TryGetDate(out var dt) ? dt : (DateTime?)null;

        public bool IsValid => Validate() == null;

        private ArgumentException? Validate()
        {
            // safe formatting for nullable ints
            var dayFmt = Day.HasValue ? Day.Value.ToString("00") : "??";
            var monthFmt = Month.HasValue ? Month.Value.ToString("00") : "??";
            var yearFmt = Year.HasValue ? Year.Value.ToString("0000") : "????";

            if (!IsValidDay)
                return new ArgumentException($"`{dayFmt}` is not a valid day");
            if (!IsValidMonth)
                return new ArgumentException($"`{monthFmt}` is not a valid month");
            if (!IsValidYear)
                return new ArgumentException($"`{yearFmt}` is not a valid year");

            // Only call DaysInMonth when all three parts are present
            if (Day.HasValue && Month.HasValue && Year.HasValue)
            {
                if (Day.Value > DateTime.DaysInMonth(Year.Value, Month.Value))
                    return new ArgumentException($"`{Day.Value}` is not a valid day in `{Year.Value}-{Month.Value:00}`");
            }

            return null;
        }

        private bool IsValidDay =>
            Day.HasValue &&
            Day.Value > 0 &&
            Day.Value <= 31;

        private bool IsValidMonth =>
            Month.HasValue &&
            Month.Value > 0 &&
            Month.Value <= 12;

        private bool IsValidYear =>
            Year.HasValue &&
            Year.Value >= DateTime.MinValue.Year &&
            Year.Value <= DateTime.MaxValue.Year;

        // Helper that produces a DateTime only when all parts are present and valid
        public bool TryGetDate(out DateTime date)
        {
            date = default;

            if (Year is int y && Month is int m && Day is int d)
            {
                // Defensive: constructing DateTime may still throw for invalid combos (e.g. Feb 29 on non-leap year),
                // but we already checked day <= DaysInMonth in Validate, so this is just extra safety.
                try
                {
                    date = new DateTime(y, m, d);
                    return true;
                }
                catch
                {
                    date = default;
                    return false;
                }
            }

            return false;
        }
    }
}