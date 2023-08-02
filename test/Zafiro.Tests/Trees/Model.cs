using System.Collections.Generic;

namespace Core.Tests.Trees;

internal record Model(string Name, IEnumerable<Model> Children);