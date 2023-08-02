using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Renci.SshNet;

namespace Zafiro.SftpFileSystem.Tests;

public class SftpFileSystemBuilder
{
    private readonly string host;
    private readonly string username;
    private readonly string password;
    private readonly int port;
    private readonly Dictionary<string, string> files = new();

    public SftpFileSystemBuilder(string host, string username, string password, int port)
    {
        this.host = host;
        this.username = username;
        this.password = password;
        this.port = port;
    }

    public async Task<IAsyncDisposable> Build()
    {
        var server = await FileSystemTests.CreateSftpServer().ConfigureAwait(false);
        await CreateFileSystem().ConfigureAwait(false);
        return server;
    }

    private async Task CreateFileSystem()
    {
        using (var client = new SftpClient(host, port, username, password))
        {
            client.Connect();

            foreach (var (path, content) in files)
            {
                var parentDir = string.Join('/', path.Split('/').SkipLast(1));
                if (!client.Exists(parentDir))
                {
                    client.CreateDirectory(parentDir);
                }

                using (var stream = client.CreateText(path))
                {
                    await stream.WriteAsync(content).ConfigureAwait(false);
                }
            }
        }
    }

    public SftpFileSystemBuilder WithFile(string path, string content = "")
    {
        files.Add(path, content);
        return this;
    }
}