namespace SftpFileSystem;

public class Credentials
{
    public Credentials(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public string Username { get; }
    public string Password { get; }
}