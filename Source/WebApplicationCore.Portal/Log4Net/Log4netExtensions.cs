using Chimera.Extensions.Logging.Log4Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging
{
    public static class Log4netExtensions
    {
        //public static ILoggerFactory AddLog4Net_Custom(this ILoggerFactory factory, string log4NetConfigFile)
        //{
        //    factory.AddProvider(new Log4NetProvider(log4NetConfigFile));
        //    return factory;
        //}

        //public static ILoggerFactory AddLog4Net_Custom(this ILoggerFactory factory)
        //{
        //    factory.AddProvider(new Log4NetProvider("log4net.config"));
        //    return factory;
        //}

        /// <summary>
        /// Adds the log4net logger to the logger factory.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <returns>The logger factory.</returns>
        /// /// <exception cref="ArgumentNullException">loggerFactory</exception>
        public static ILoggerFactory AddLog4Net_Custom(this ILoggerFactory loggerFactory)
        {
            return AddLog4Net_Custom(loggerFactory, Log4NetSettings.Default);
        }

        /// <summary>
        /// Adds the log4net logger to the logger factory.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration container.</param>
        /// <returns>The logger factory.</returns>
        /// <exception cref="ArgumentNullException">loggerFactory or configuration</exception>
        public static  ILoggerFactory AddLog4Net_Custom(this ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return AddLog4Net_Custom(loggerFactory, new ConfigurationLog4NetSettings(configuration));
        }

        /// <summary>
        /// Adds the log4net logger to the logger factory.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="settings">The log4net settings.</param>
        /// <returns>The logger factory.</returns>
        /// <exception cref="ArgumentNullException">loggerFactory or settings</exception>
        public static ILoggerFactory AddLog4Net_Custom(this ILoggerFactory loggerFactory, ILog4NetSettings settings)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var container = new Log4NetContainer(settings);
            container.Initialize();

            log4net.GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;
            loggerFactory.AddProvider(new Log4NetProvider(container));

            return loggerFactory;
        }
    }
}
