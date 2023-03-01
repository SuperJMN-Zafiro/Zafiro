using System;

namespace Zafiro.Core.Mixins;

public record ProgressSnapshot(double CurrentProgress, DateTimeOffset FinishTime, TimeSpan RemainingTime);