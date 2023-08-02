using System.Collections.Generic;

namespace Zafiro.Tests.Trees;

internal record Model(string Name, IEnumerable<Model> Children);