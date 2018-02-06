using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Common
{
    public interface IDateTimeService
    {
        /// <summary>
        /// Gets the current time. 
        /// </summary>
        /// <returns></returns>
        DateTime Now();

        /// <summary>
        /// Gets the previous submission period.
        /// </summary>
        /// <param name="referenceDateTime"></param>
        /// <returns></returns>
        DateTime GetPreviousSubmissionPeriod(DateTime referenceDateTime);

        /// <summary>
        /// Gets the current submission period.
        /// </summary>
        /// <param name="referenceDateTime">The reference date.</param>
        /// <returns></returns>
        DateTime GetSubmissionPeriod(DateTime referenceDateTime);

        /// <summary>
        /// Gets the start of the period for the given reference date time.
        /// </summary>
        /// <param name="referenceDateTime"></param>
        /// <returns></returns>
        DateTime GetSubmissionPeriodStart(DateTime referenceDateTime);

        /// <summary>
        /// Gets the submission periods.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IEnumerable<DateTime> GetSubmissionPeriods(DateTime start, DateTime end);

        /// <summary>
        /// Gets the submission periods in the given year.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        IEnumerable<DateTime> GetSubmissionPeriods(int year);

        /// <summary>
        /// Gets whether the given date represents a valid period end.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        bool IsValidPeriod(DateTime dateTime);
    }
}
