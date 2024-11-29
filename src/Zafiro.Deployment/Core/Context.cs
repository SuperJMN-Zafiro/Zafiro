using CSharpFunctionalExtensions;
using Serilog;

namespace Zafiro.Deployment.Core;

public class Context
{
    public Context(IDotnet dotnet, ICommand command, Maybe<ILogger> logger, IHttpClientFactory httpClientFactory)
    {
        Dotnet = dotnet;
        Command = command;
        Logger = logger;
        HttpClientFactory = httpClientFactory;
    }

    public Maybe<ILogger> Logger { get; }
    public IHttpClientFactory HttpClientFactory { get; }
    public IDotnet Dotnet { get; }
    public ICommand Command { get; }
}