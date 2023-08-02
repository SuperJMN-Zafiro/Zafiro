using System;

namespace Zafiro.Mixins;

public record ProgressSnapshot(DateTimeOffset FinishTime, TimeSpan RemainingTime);