using BneDev.TimesheetNinja.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Common
{

    /// <summary>
    /// Represents an error that occurs in the business layer, such as the results of applying business rules (validation results). 
    /// This should be handled and recoverable (that is, assuming the errors are addressed in a subsequent request).
    /// </summary>
    [Serializable]
    public class BusinessException : ApiException
    {
        private KeyValuePair<string, string>[] errors;

        public BusinessException(string message, params KeyValuePair<string, string>[] errors)
            : base(message)
        {
            this.errors = errors;
        }

        public IReadOnlyCollection<KeyValuePair<string, string>> Errors => errors;

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info = info ?? throw new ArgumentNullException(nameof(info));

            info.AddValue(nameof(Errors), Errors);
        }
    }
}
