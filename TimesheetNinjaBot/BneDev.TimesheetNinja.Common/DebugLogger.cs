using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Common
{
    public class DebugLogger : ILogger
    {
        public void WriteInfo(string message, params object[] parameters)
        {
            Debug.WriteLine(message, parameters);
        }
    }
}
