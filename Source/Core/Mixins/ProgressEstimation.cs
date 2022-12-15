using System;

namespace Zafiro.Core.Mixins;

public record ProgressEstimation(double CurrentProgress, DateTimeOffset FinishTime, TimeSpan RemainingTime);