using System;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class DateModel
    {
        public DateModel()
        {
        }

        public DateModel(DateTime date) : this(date.Year, date.Month, date.Day)
        {
        }

        public DateModel(int year, int month, int day) =>
            (Year, Month, Day) = (year, month, day);

        public virtual int? Day { get; set; }
        public virtual int? Month { get; set; }
        public virtual int? Year { get; set; }

        public DateTime Date =>
            IsValid ? new DateTime(Year.Value, Month.Value, Day.Value) : default;

        public bool IsValid => Validate() == null;

        private Exception Validate()
        {
            if (!IsValidDay)
                return new ArgumentException($"`{Day:00}` is not a valid day");
            if (!IsValidMonth)
                return new ArgumentException($"`{Month:00}` is not a valid month");
            if (!IsValidYear)
                return new ArgumentException($"`{Year:0000}` is not a valid year");
            if (Day > DateTime.DaysInMonth(Year.Value, Month.Value))
                return new ArgumentException($"`{Day}` is not a valid day in `{Year}-{Month:00}`");
            return null;
        }

        private bool IsValidDay =>
            Day.HasValue &&
            Day > 0 &&
            Day <= 31;

        private bool IsValidMonth =>
            Month.HasValue &&
            Month > 0 &&
            Month <= 12;

        private bool IsValidYear =>
            Year.HasValue &&
            Year >= DateTime.MinValue.Year &&
            Year <= DateTime.MaxValue.Year;
    }
}
