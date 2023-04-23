using SynthesizerEngine.Pattern;

namespace SynthesizerEngine.Core;

public class SchedulerEvent
{
    public double Time { get; set; }
    public Action<List<object>>? Callback { get; set; }
    public List<PatternBase> Patterns { get; set; }
    public object? DurationPattern { get; set; }

    public SchedulerEvent(double time = 0, Action<List<object>>? callback = null, List<PatternBase>? patterns = null, object? durationPattern = null)
    {
        Time = time;
        Callback = callback;
        Patterns = patterns ?? new List<PatternBase>();
        DurationPattern = durationPattern;
    }

}