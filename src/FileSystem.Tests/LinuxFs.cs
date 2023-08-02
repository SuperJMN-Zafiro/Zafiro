using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace FileSystem.Tests;

public class LinuxFs : MockFileSystem
{
    private IPath? path;

    public LinuxFs(Dictionary<string, MockFileData> files) : base(files)
    {
        AddDirectory("");
    }

    public override IPath Path
    {
        get
        {
            if (path != null) return path;

            path = new LinuxPath(this);
            return path;
        }
    }
}