using BneDev.TimesheetNinjaBot.BffServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace BneDev.TimesheetNinjaBot.Controllers
{
    public class DefaultController : ApiController
    {
        private TimesheetNinjaBotMockBffService timesheetNinjaBotMockBffService;

        public DefaultController(TimesheetNinjaBotMockBffService timesheetNinjaBotMockBffService)
        {
            this.timesheetNinjaBotMockBffService = timesheetNinjaBotMockBffService;
        }

        [HttpGet()]
        [Route("api/demo/data/raw")]
        public async Task<HttpResponseMessage> GetDemoData()
        {
            var timesheetDataRaw = timesheetNinjaBotMockBffService.TimesheetDataRaw;

            return await Task.FromResult(new HttpResponseMessage()
            {
                Content = new StringContent(timesheetDataRaw, Encoding.UTF8, "application/json"),
                StatusCode = HttpStatusCode.OK
            });
        }

        [HttpGet()]
        [Route("api/demo/data/timesheet/all/tasksummary")]
        public async Task<IHttpActionResult> GetTasksData()
        {
            var timesheetData = timesheetNinjaBotMockBffService.TimesheetData;

            var timesheetTaskSummary = timesheetData.Timesheets.SelectMany(x => x.GetDailyTimeEntries());

            return await Task.FromResult(Json(timesheetTaskSummary));
        }

        private static readonly string NonAlphaNumeric = @"[^\w\d\s]";
        private static readonly char[] Spaces = new char[] { ' ' };

        [HttpGet()]
        [Route("api/demo/projectsearchlistentity")]
        public async Task<HttpResponseMessage> GetProjectSearchListEntity()
        {
            var timesheetData = timesheetNinjaBotMockBffService.TimesheetData;

            var projectCodesStartsWithIndex = timesheetData.Projects.Select(x => x.Code.Substring(0, 4)).ToArray();
            var projectCodesIndex = timesheetData.Projects.Select(x => x.Code);
            var projectDescriptions = timesheetData.Projects.Select(x => x.Description);
            var projectDescriptionsNormalized = projectDescriptions.Select(x => Regex.Replace(x, NonAlphaNumeric, String.Empty));
            var projectDescriptionsNormalizedTokenized = projectDescriptionsNormalized.SelectMany(x => x.Split(Spaces, StringSplitOptions.RemoveEmptyEntries));

            var tokens = new List<string>();
            tokens.AddRange(projectCodesStartsWithIndex);
            tokens.AddRange(projectCodesIndex);
            tokens.AddRange(projectDescriptions);
            tokens.AddRange(projectDescriptionsNormalized);
            tokens.AddRange(projectDescriptionsNormalizedTokenized);

            var bigString = String.Join(",", tokens);

            return await Task.FromResult(new HttpResponseMessage()
            {
                Content = new StringContent(bigString, Encoding.UTF8),
                StatusCode = HttpStatusCode.OK
            });
        }
    }
}
