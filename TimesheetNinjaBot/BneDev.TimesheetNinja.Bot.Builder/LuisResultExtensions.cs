
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BneDev.TimesheetNinja.Bot.Builder.Model;
using BneDev.TimesheetNinja.Common;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Luis;

namespace BneDev.TimesheetNinja.Bot.Builder
{
    // https://docs.microsoft.com/en-us/azure/cognitive-services/luis/pre-builtentities#builtindatetimeV2
    public static class LuisResultExtensions
    {
        public static bool TryGetDuration(this LuisResult luisResult, out TimeSpan duration)
        {
            if (luisResult.TryFindEntity(EntityIds.BuiltInDateTimeV2Duration, out EntityRecommendation recommendation))
            {
                var values = ((IList<object>)recommendation.Resolution["values"]);
                var value = (values[0]) as IDictionary<string, object>;
                var durationSeconds = double.Parse(value["value"] as string);

                duration = TimeSpan.FromSeconds(durationSeconds);
                return true;
            }
            duration = TimeSpan.Zero;
            return false;
        }

        public static bool TryGetDateTimeRange(this LuisResult luisResult, out (DateTime start, DateTime end, double? hours) dateRange)
        {
            if (luisResult.TryFindEntity(EntityIds.BuiltInDateTimeV2DateTimeRange, out EntityRecommendation recommendation) && recommendation.Resolution != null)
            {
                var values = ((IList<object>)recommendation.Resolution["values"]);
                var value = (values[0]) as IDictionary<string, object>;
                if (value != null && value.Count == 4)
                {
                    DateTime start = DateTimeService.ParseIso8601DateTime(value["start"] as string);
                    DateTime end = DateTimeService.ParseIso8601DateTime(value["end"] as string);
                    // Simplificaiton: Take the hour component only when start == end
                    // So handling simple date time ranges, such as "Yesterday morning"
                    double? hours = start.Date == end.Date ? (double?)(end.TimeOfDay - start.TimeOfDay).TotalHours : null;
                    dateRange = (start, end, hours);
                    return true;
                }
            }
            dateRange = default;
            return false;
        }

        public static bool TryGetDateRange(this LuisResult luisResult, out (DateTime start, DateTime end) dateRange)
        {
            if (luisResult.TryFindEntity(EntityIds.BuiltInDateTimeV2DateRange, out EntityRecommendation recommendation) && recommendation.Resolution != null)
            {
                var values = ((IList<object>)recommendation.Resolution["values"]);
                var value = (values[0]) as IDictionary<string, object>;
                if (value != null && value.Count == 4)
                {
                    DateTime start = DateTimeService.ParseIso8601Date(value["start"] as string);
                    DateTime end = DateTimeService.ParseIso8601Date(value["end"] as string);
                    dateRange = (start, end);
                    return true;
                }
            }
            dateRange = default;
            return false;
        }

        public static bool TryGetPeriod(this LuisResult luisResult, IDateTimeService dateTimeService, DateTime referenceDateTime, out DateTime periodEnd)
        {
            if (luisResult.TryFindEntity(EntityIds.Period, out EntityRecommendation recommendation) && recommendation.Resolution != null)
            {
                var values = ((IList<object>)recommendation.Resolution["values"]);
                Period period;
                if (Enum.TryParse(values[0] as string, true, out period))
                {
                    switch (period)
                    {
                        default:
                        case Period.Current:
                            periodEnd = dateTimeService.GetSubmissionPeriod(referenceDateTime);
                            break;

                        case Period.Previous:
                            periodEnd = dateTimeService.GetPreviousSubmissionPeriod(referenceDateTime);
                            break;
                    }
                    return true;
                }
            }
            periodEnd = default;
            return false;
        }

        internal const string ReimbursedNormalizedValue = "paid";
        public static bool TryGetExpensePeriod(this LuisResult luisResult, IDateTimeService dateTimeService, DateTime referenceDateTime, out DateTime periodEnd)
        {
            if (luisResult.TryFindEntity(EntityIds.Reimbursed, out EntityRecommendation recommendation) && recommendation.Resolution != null)
            {
                var values = ((IList<object>)recommendation.Resolution["values"]);
                if (String.Equals(values[0] as string, ReimbursedNormalizedValue, StringComparison.OrdinalIgnoreCase))
                {
                    periodEnd = dateTimeService.GetPreviousSubmissionPeriod(referenceDateTime);
                    return true;
                }
            }
            periodEnd = default;
            return false;
        }

        public static bool TryGetDate(this LuisResult luisResult, out DateTime date)
        {
            if (luisResult.TryFindEntity(EntityIds.BuiltInDateTimeV2Date, out EntityRecommendation dateRecommendation) && dateRecommendation.Resolution != null)
            {
                var values = ((IList<object>)dateRecommendation.Resolution["values"]);
                var value = (values[0]) as IDictionary<string, object>;
                date = DateTimeService.ParseIso8601Date(value["value"] as string);

                return true;
            }
            date = default;
            return false;
        }

        private static bool TryGetWbsStanding(this LuisResult luisResult, out string wbsStanding)
        {
            if (luisResult.TryFindEntity(EntityIds.WbsStanding, out EntityRecommendation recommendation) && recommendation.Resolution != null)
            {
                var values = ((IList<object>)recommendation.Resolution["values"]);

                wbsStanding = values[0] as string;
                return true;
            }
            wbsStanding = null;
            return false;
        }

        public static bool TryGetWbs(this LuisResult luisResult, out string wbs)
        {
            if (luisResult.TryGetWbsStanding(out wbs))
            {
                return true;
            }
            else
            {
                if (luisResult.TryFindEntity(EntityIds.Wbs, out EntityRecommendation recommendation))
                {
                    // Just take the plain-old WBS: 
                    // _wbs_ -> wbs
                    // _ wbs _ -> wbs
                    wbs = recommendation.Entity.Trim('_').Trim();
                    return true;
                }

                wbs = null;
                return false;
            }
        }
    }
}
