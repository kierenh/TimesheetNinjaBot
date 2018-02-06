using BneDev.TimesheetNinja.Common;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Bot.Api
{
    public interface IApis
    {
        ITimeApi TimeApi(AuthToken authToken);

        IProjectApi ProjectApi(AuthToken authToken);

        IExpenseApi ExpenseApi(AuthToken authToken);

        IAuthApi AuthApi();
    }
}
