﻿using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Tests;

public class PathTests
{
    [Fact]
    public void Test()
    {
        var path = (ZafiroPath) "usr/bin/text.txt";
        var parents = path.Parents();

        parents.Select(x => x.ToString()).Should().BeEquivalentTo("", "usr", "usr/bin");
    }
}