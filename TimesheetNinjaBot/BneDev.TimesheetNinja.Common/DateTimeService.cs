using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Common
{
    /// <summary>
    /// A date time service that supports timesheeting.
    /// </summary>
    public class DateTimeService : IDateTimeService
    {
        public static DateTime ParseIso8601Date(string value, string delimeter = "-")
        {
            return DateTime.ParseExact(value, $"yyyy{delimeter}MM{delimeter}dd", CultureInfo.InvariantCulture);
        }

        public static DateTime ParseIso8601DateTime(string value, string delimeter = "-")
        {
            return DateTime.ParseExact(value, $"yyyy{delimeter}MM{delimeter}dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        public DateTimeService()
        {
        }

        /// <summary>
        /// Overrides the system date time (to a fixed point in time).
        /// </summary>
        public DateTime? SystemDateTime { get; set; }

        public virtual DateTime Now()
        {
            return SystemDateTime.GetValueOrDefault(DateTime.Now);
        }

        internal const int MidMonthDay = 15;

        public DateTime GetPreviousSubmissionPeriod(DateTime referenceDateTime)
        {
            var currentPeriod = GetSubmissionPeriod(referenceDateTime);
            if (currentPeriod.Day <= MidMonthDay)
            {
                // Go back to previous month (which may be previous year)
                currentPeriod = currentPeriod.AddMonths(-1);
                // Work out how many days in year/month
                currentPeriod = new DateTime(currentPeriod.Year, currentPeriod.Month, DateTime.DaysInMonth(currentPeriod.Year, currentPeriod.Month));
            }
            else
            {
                currentPeriod = new DateTime(currentPeriod.Year, currentPeriod.Month, MidMonthDay);
            }
            return currentPeriod;
        }

        /// <summary>
        /// Gets the current period.
        /// </summary>
        /// <param name="referenceDateTime">The reference date.</param>
        /// <returns></returns>
        public DateTime GetSubmissionPeriod(DateTime referenceDateTime)
        {
            // Periods do not split across years
            return referenceDateTime.Day > MidMonthDay ? new DateTime(referenceDateTime.Year, referenceDateTime.Month, DateTime.DaysInMonth(referenceDateTime.Year, referenceDateTime.Month)) : new DateTime(referenceDateTime.Year, referenceDateTime.Month, MidMonthDay);
        }

        public DateTime GetSubmissionPeriodStart(DateTime referenceDateTime)
        {
            var periodStart = GetPreviousSubmissionPeriod(referenceDateTime).AddDays(1.0);
            return periodStart;
        }

        public IEnumerable<DateTime> GetSubmissionPeriods(DateTime start, DateTime end)
        {
            return (from x in new[] { start.Year, end.Year }
                    from period in GetSubmissionPeriods(x)
                    where period >= start && period <= end
                    select period);
        }

        public IEnumerable<DateTime> GetSubmissionPeriods(int year)
        {
            int periodsInYear = 24;

            var periods = (from i in Enumerable.Range(1, periodsInYear)
                           let month = (i + 1) / 2
                           let day = (i % 2) == 1 ? MidMonthDay : DateTime.DaysInMonth(year, month)
                           select new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Local));

            return periods;
        }

        public bool IsValidPeriod(DateTime dateTime)
        {
            return dateTime.Day == MidMonthDay || dateTime.Day == DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
        }
    }
}
