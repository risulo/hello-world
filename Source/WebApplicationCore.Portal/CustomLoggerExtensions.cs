using Microsoft.Extensions.Logging.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging
{
    public static class CustomLoggerExtensions
    {
        private static readonly Func<object, Exception, string> _messageFormatter = new Func<object, Exception, string>(CustomLoggerExtensions.MessageFormatter);

        public static void LogCritical(this ILogger logger, Exception exception, string message, IDictionary<string, object> parameters = null, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            string parameterList = @"Parameter listing:" + Environment.NewLine;

            foreach (KeyValuePair<string, object> kvp in parameters)
            {
                string paramValue = (kvp.Value == null) ? "null" : kvp.Value.ToString();
                parameterList += string.Format("\tName: {0}, \t Value: {1}", kvp.Key, paramValue) + Environment.NewLine;
            }

            message = message + Environment.NewLine + parameterList;

            if (exception.Data.Keys.Count > 0)
            {
                string exceptionAdditionalData = "\nException additional data:";

                foreach (object key in exception.Data.Keys)
                {
                    string keyString = key.ToString();
                    object value = exception.Data[key];
                    string valueString = value.ToString();

                    exceptionAdditionalData += string.Format("\n\tKey: {0}, \tValue: {1}", keyString, valueString);
                }

                message += exceptionAdditionalData;
            }

            logger.Log<object>(LogLevel.Critical, 0, new FormattedLogValues(message, args), exception, CustomLoggerExtensions._messageFormatter);
        }

        private static string MessageFormatter(object state, Exception error)
        {
            //return state.ToString() + "\nStack Trace: " + error.ToString();
            return state.ToString();
        }
    }
}
