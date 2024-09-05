using Renci.SshNet;

namespace Zafiro.FileSystem.Sftp;

public static class SftpClientExtensions
{
    public static Task UploadFileAsync(this SftpClient sftpClient, string remotePath, Stream stream)
    {
        return Task.Factory.FromAsync(
            (callback, state) => sftpClient.BeginUploadFile(stream, remotePath, callback, state),
            sftpClient.EndUploadFile,
            null
        );
    }

    public static Task DownloadFileAsync(this SftpClient sftpClient, string remotePath, Stream stream)
    {
        return Task.Factory.FromAsync(
            (callback, state) => sftpClient.BeginDownloadFile(remotePath, stream, callback, state),
            sftpClient.EndDownloadFile,
            null);
    }

    public static Task<IEnumerable<Renci.SshNet.Sftp.SftpFile>> ListDirectoryAsync(this SftpClient sftpClient, string remotePath)
    {
        return Task.Factory.FromAsync(
            (callback, state) => sftpClient.BeginListDirectory(remotePath, callback, state),
            sftpClient.EndListDirectory,
            null);
    }
}