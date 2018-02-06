using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Common
{
    [Serializable]
    public class ApiException : Exception
    {
        public ApiException(string message)
            : this(message, null)
        {
        }

        public ApiException(string message, Exception e)
            : base(message, e)
        {
        }
    }
}
