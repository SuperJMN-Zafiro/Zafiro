using System;

namespace Zafiro.Core.Mixins;

public record ProgressSnapshot(DateTimeOffset FinishTime, TimeSpan RemainingTime);