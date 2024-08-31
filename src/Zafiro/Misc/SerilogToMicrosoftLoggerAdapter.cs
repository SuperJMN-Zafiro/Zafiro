using System;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace Zafiro.Misc;

public class SerilogToMicrosoftLoggerAdapter : ILogger
{
    private readonly Serilog.ILogger logger;

    public SerilogToMicrosoftLoggerAdapter(Serilog.ILogger logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logger.IsEnabled(ConvertToSerilogLevel(logLevel));
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var serilogLogLevel = ConvertToSerilogLevel(logLevel);
        if (formatter != null)
        {
            logger.Write(serilogLogLevel, exception, formatter(state, exception));
        }
        else
        {
            logger.Write(serilogLogLevel, exception, state.ToString());
        }
    }

    private static LogEventLevel ConvertToSerilogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => LogEventLevel.Verbose,
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Critical => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
}