using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Pattern;
using SynthesizerEngine.Util;

namespace SynthesizerEngine.Core;

public class Scheduler : PassThroughNode
{
    private double _bpm;
    private readonly BinaryHeapPriorityQueue<SchedulerEvent> _queue;
    private double _time;
    private double _beat;
    private double _beatInBar;
    private double _bar;
    private double _seconds;
    private readonly double _beatsPerBar;
    private double _lastBeatTime;
    private double _beatLength;

    public Scheduler(Provider lib, int bpm = 120) : base(lib, 1, 1)
    {
        _bpm = bpm;
        _queue = new BinaryHeapPriorityQueue<SchedulerEvent>((a, b) => a.Time.CompareTo(b.Time));

        _time = 0;
        _beat = 0;
        _beatInBar = 0;
        _bar = 0;
        _seconds = 0;
        _beatsPerBar = 0;

        _lastBeatTime = 0;
        _beatLength = 60 / _bpm * AudioProvider.WaveFormat.SampleRate;
    }

    public void SetTempo(int bpm)
    {
        _bpm = bpm;
        _beatLength = 60 / _bpm * AudioProvider.WaveFormat.SampleRate;
    }

    SchedulerEvent AddRelative(int beats, Action<List<object>>? callback = null)
    {
        var newEvent = new SchedulerEvent(_time + beats * _beatLength, callback);
        _queue.Enqueue(newEvent);
        return newEvent;
    }


    public SchedulerEvent? AddAbsolute(double beat, Action<List<object>>? callback)
    {
        if (beat < _beat || (Math.Abs(beat - _beat) < double.Epsilon && _time > _lastBeatTime))
        {
            return null;
        }

        var newEvent = new SchedulerEvent(_lastBeatTime + (beat - _beat) * _beatLength, callback);
        _queue.Enqueue(newEvent);
        return newEvent;
    }

    public SchedulerEvent Play(List<PatternBase> patterns, object? durationPattern, Action<List<object>>? callback)
    {
        var newEvent = new SchedulerEvent(AudioProvider.GetWriteTime(), callback, patterns, durationPattern);
        _queue.Enqueue(newEvent);
        return newEvent;
    }

    public SchedulerEvent? PlayAbsolute(double beat, List<PatternBase> patterns, object? durationPattern,
        Action<List<object>>? callback)
    {
        if (beat < _beat || (Math.Abs(beat - _beat) < double.Epsilon && _time > _lastBeatTime))
        {
            return null;
        }

        var newEvent =
            new SchedulerEvent(_lastBeatTime + (beat - _beat) * _beatLength, callback, patterns, durationPattern);
        _queue.Enqueue(newEvent);
        return newEvent;
    }


    private void Remove(SchedulerEvent eventToRemove)
    {
        _queue.Remove(eventToRemove);
    }

    public void Stop(SchedulerEvent eventToStop)
    {
        Remove(eventToStop);
    }

    public override void Tick()
    {
        base.Tick();

        TickClock();

        while (_queue.Count > 0 && _queue.Peek().Time <= _time)
        {
            var eventToProcess = _queue.Dequeue();
            ProcessEvent(eventToProcess);
        }
    }

    private void TickClock()
    {
        _time += 1;
        _seconds = _time / AudioProvider.WaveFormat.SampleRate;

        if (!(_time >= _lastBeatTime + _beatLength)) return;

        _beat += 1;
        _beatInBar += 1;

        if (Math.Abs(_beatInBar - _beatsPerBar) < double.Epsilon)
        {
            _bar += 1;
            _beatInBar = 0;
        }

        _lastBeatTime += _beatLength;
    }

    private void ProcessEvent(SchedulerEvent eventToProcess)
    {
        var durationPattern = eventToProcess.DurationPattern;
        if (durationPattern != null)
        {
            var args = new List<object>();
            var numberOfPatterns = eventToProcess.Patterns.Count;
            for (var i = 0; i < numberOfPatterns; i++)
            {
                var pattern = eventToProcess.Patterns[i];
                var value = pattern.Next();
                if (value != null)
                {
                    args.Add(value);
                }
                else
                {
                    // Null value for an argument, so don't process the
                    // callback or add any further events
                    return;
                }
            }

            eventToProcess.Callback?.Invoke(args);


            object? duration;
            if (durationPattern is PatternBase pb)
            {
                duration = pb.Next();
            }
            else
            {
                duration = durationPattern;
            }

            if (duration != null)
            {
                var durationValue = PatternBase.ConvertValueToNumber(duration) ?? 0;

                eventToProcess.Time += durationValue * _beatLength;
                _queue.Enqueue(eventToProcess);
            }


        }
        else
        {
            eventToProcess.Callback?.Invoke(new List<object>());
        }
    }
}
