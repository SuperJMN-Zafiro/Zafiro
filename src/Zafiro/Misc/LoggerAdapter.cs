using System;
using Microsoft.Extensions.Logging;

namespace Zafiro.Misc;

public class LoggerAdapter : ILogger
{
    private readonly Serilog.ILogger logger;

    public LoggerAdapter(Serilog.ILogger logger)
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

    private static Serilog.Events.LogEventLevel ConvertToSerilogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => Serilog.Events.LogEventLevel.Verbose,
            LogLevel.Debug => Serilog.Events.LogEventLevel.Debug,
            LogLevel.Information => Serilog.Events.LogEventLevel.Information,
            LogLevel.Warning => Serilog.Events.LogEventLevel.Warning,
            LogLevel.Error => Serilog.Events.LogEventLevel.Error,
            LogLevel.Critical => Serilog.Events.LogEventLevel.Fatal,
            _ => Serilog.Events.LogEventLevel.Information,
        };
    }
}