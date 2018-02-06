using BneDev.TimesheetNinja.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Bot.Api
{
    public interface IAuthApi
    {
        Task<AuthToken> AuthTokenFor(string accessToken);

        Task<bool> IsAccessTokenActive(AuthToken authToken);
    }
}
