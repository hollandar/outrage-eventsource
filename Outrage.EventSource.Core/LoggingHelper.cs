using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventSource.Core
{
    public static class LoggingHelper
    {
        public static Func<(string message, object?[]? parameters)> LogFormat(this string msg, params object?[] param)
        {
            return () => (msg, param);
        }

        public static Func<(string message, object?[]? parameters)> LogFormat(this string msg, Func<object?> param)
        {
            return () => (msg, new object?[] { param() });
        }

        public static Func<(string message, object?[]? parameters)> LogFormat(this string msg, Func<object?[]> param)
        {
            return () => (msg, param());
        }

        public static void FastLog(this ILogger? logger, LogLevel level, Func<(string msg, object?[]? parameters)> msg)
        {
            if (logger?.IsEnabled(level) ?? false)
            {
                var props = msg();
                logger.Log(level, props.msg, props.parameters ?? Array.Empty<object>());
            }
        }

        public static void FastLogException(this ILogger? logger, LogLevel level, Exception e, Func<(string msg, object?[]? parameters)>? msg = null)
        {
            if (logger?.IsEnabled(level) ?? false)
            {
                if (msg != null)
                {
                    var props = msg();
                    logger.Log(level, e, props.msg, props.parameters ?? Array.Empty<object>());
                }
                else
                {
                    logger.Log(level, e, String.Empty);
                }
            }
        }
    }
}
