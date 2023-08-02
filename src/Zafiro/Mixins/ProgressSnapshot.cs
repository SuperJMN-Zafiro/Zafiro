using System;

namespace Zafiro.Zafiro.Mixins;

public record ProgressSnapshot(DateTimeOffset FinishTime, TimeSpan RemainingTime);