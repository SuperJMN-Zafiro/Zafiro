using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Renci.SshNet;

namespace SftpFileSystem.Tests;

public class Builder
{
    private readonly string host;
    private readonly string username;
    private readonly string password;
    private readonly int port;
    private readonly Dictionary<string, string> files = new();

    public Builder(string host, string username, string password, int port)
    {
        this.host = host;
        this.username = username;
        this.password = password;
        this.port = port;
    }

    public async Task<IAsyncDisposable> Build()
    {
        var server = await FileSystemTests.CreateSftpServer();
        await CreateFileSystem();
        return server;
    }

    private async Task CreateFileSystem()
    {
        using (var client = new SftpClient(host, port, username, password))
        {
            client.Connect();

            foreach (var (path, content) in files)
            {
                using (var stream = client.CreateText(path))
                {
                    await stream.WriteAsync(content);
                }
            }
        }
    }

    public Builder WithFile(string path, string content = "")
    {
        files.Add(path, content);
        return this;
    }
}