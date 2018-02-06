using BneDev.TimesheetNinjaBot;
using BneDev.TimesheetNinjaBot.Models;
using BneDev.TimesheetNinja.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BneDev.TimesheetNinja.Bot.Api;
using BneDev.TimesheetNinja.Bot.Api.Models;

namespace BneDev.TimesheetNinjaBot.BffServices
{
    public class TimesheetNinjaBotMockBffService : ITimeApi, IExpenseApi, IProjectApi, IAuthApi
    {
        private CommonServices commonServices;
        public TimesheetData TimesheetData { get; private set; }
        public string TimesheetDataRaw { get; private set; }

        private string ReadTimesheetData(string appDataPath)
        {
            string timesheetDataPath = Path.Combine(appDataPath, "TimesheetData.json");
            var timesheetDataRaw = File.ReadAllText(timesheetDataPath);
            return timesheetDataRaw;
        }

        public TimesheetNinjaBotMockBffService(CommonServices commonServices, string appDataPath, bool useInMemory = false)
        {
            this.commonServices = commonServices;
            if (useInMemory)
            {

            }
            else
            {
                TimesheetDataRaw = ReadTimesheetData(appDataPath);
                TimesheetData = JsonConvert.DeserializeObject<TimesheetData>(TimesheetDataRaw);
            }
        }

        async Task IProjectApi.Add(string projectCode)
        {
            commonServices.Logger().WriteInfo($"Adding project {projectCode}");
            await Task.CompletedTask;
        }

        async Task ITimeApi.Add(AddTimeEntry addTimeEntry)
        {
            commonServices.Logger().WriteInfo($"Adding time entry {addTimeEntry.WbsCode} / {addTimeEntry.FromDate} / {addTimeEntry.ToDate} / {addTimeEntry.Hours}");
            await Task.CompletedTask;
        }

        async Task<ITimesheet> ITimeApi.Get(DateTime periodEnd)
        {
            AssertReferenceDateIsValid(periodEnd);

            var dateTimeService = commonServices.DateTimeService();
            var isValidPeriod = dateTimeService.IsValidPeriod(periodEnd);
            if (isValidPeriod)
            {
                var timesheet = TimesheetData.Timesheets.SingleOrDefault(x => DateTimeService.ParseIso8601Date(x.PeriodEnd).Date == periodEnd.Date);
                return await Task.FromResult(timesheet);
            }
            else
            {
                throw new BusinessException($"{periodEnd} is not a valid period.");
            }
        }

        async Task<IExpenseSheet> IExpenseApi.Get(DateTime periodEnd)
        {
            AssertReferenceDateIsValid(periodEnd);

            var dateTimeService = commonServices.DateTimeService();
            var isValidPeriod = dateTimeService.IsValidPeriod(periodEnd);
            if (isValidPeriod)
            {
                var expenseSheet = TimesheetData.ExpenseSheets.SingleOrDefault(x => x.PeriodEnd.Date == periodEnd.Date);
                return await Task.FromResult(expenseSheet);
            }
            else
            {
                throw new BusinessException($"{periodEnd} is not a valid period.");
            }
        }

        async Task<IEnumerable<IProject>> IProjectApi.GetAll()
        {
            return await Task.FromResult(TimesheetData.Projects.ToArray());
        }

        async Task<(ITimesheet timesheet, bool isSubmitted)> ITimeApi.IsSubmitted(DateTime referenceDate)
        {
            AssertReferenceDateIsValid(referenceDate);

            var dateTimeService = commonServices.DateTimeService().GetSubmissionPeriod(referenceDate);
            var timesheet = this.TimesheetData.Timesheets.Where(x => DateTimeService.ParseIso8601Date(x.PeriodEnd).Date == referenceDate.Date).Single();
            return await Task.FromResult((timesheet, String.Equals(timesheet.Status, "processed", StringComparison.OrdinalIgnoreCase)));
        }

        async Task<ITimesheet> ITimeApi.StraightEights(DateTime periodEnd, string taskNumber, string taskDescription, double hours, bool submit)
        {
            AssertReferenceDateIsValid(periodEnd);

            commonServices.Logger().WriteInfo($"Assigning {hours}h to {taskNumber} \\ {taskDescription} and submitting!");
            var timesheet = this.TimesheetData.Timesheets.Where(x => DateTimeService.ParseIso8601Date(x.PeriodEnd).Date == periodEnd.Date).Single();
            return await Task.FromResult(timesheet);
        }

        async Task ITimeApi.Submit()
        {
            commonServices.Logger().WriteInfo($"Timesheet submitted, until next time grasshopper!");
            await Task.CompletedTask;
        }

        async Task<AuthToken> IAuthApi.AuthTokenFor(string accessToken)
        {
            return await Task.FromResult(new AuthToken()
            {
                AccessToken = "Teh Access Token",
                Id = "dorothy.mctimesheety",
                UserDisplayName = "Dorothy McTimesheety"
            });
        }

        async Task<bool> IAuthApi.IsAccessTokenActive(AuthToken authToken)
        {
            return await Task.FromResult(true);
        }

        public DateTime DateTimeMin { get; set; }
        public DateTime DateTimeMax { get; set; }

        public void AssertReferenceDateIsValid(DateTime referenceDateTime)
        {
            var isValid = referenceDateTime.Date >= DateTimeMin.Date && referenceDateTime.Date <= DateTimeMax.Date;
            var dateTimeService = this.commonServices.DateTimeService();
            var now = dateTimeService.Now();

            if (!isValid)
            {
                string message = $"You tried to enter {referenceDateTime.Date:dd/MM/yyyy}, but this demo has a limited data set. The clock is fixed to {now:dd/MM/yyyy}, and, time and expenses must be within {DateTimeMin:dd/MM/yyyy} and {DateTimeMax:dd/MM/yyyy}.";
                throw new ApiException(message);
            }
        }
    }
}
