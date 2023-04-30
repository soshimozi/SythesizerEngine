using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class Envelope : Node
{
    public Automation Gate { get; }

    private readonly Automation[] _levels;
    private readonly Automation[] _times;

    private readonly int? _releaseStage;

    private int? _stage;
    private int? _time;
    private int? _changeTime;

    private double _level;
    private double _delta;
    private bool _gateOn;

    public event EventHandler<EventArgs>? Complete;

    protected Envelope(IAudioProvider provider, double? gate, IReadOnlyList<double> levels, IReadOnlyList<double> times, int? releaseStage = null)
        : base(provider, 1, 1)
    {
        Gate = new Automation(this, 0, gate ?? 1);

        _levels = new Automation[levels.Count];
        for (var i = 0; i < levels.Count; i++)
        {
            _levels[i] = new Automation(this, null, levels[i]);
        }

        _times = new Automation[times.Count];
        for (var i = 0; i < times.Count; i++)
        {
            _times[i] = new Automation(this, null, times[i]);
        }

        _releaseStage = releaseStage;

        _stage = null;
        _time = null;
        _changeTime = null;

        _level = _levels[0].GetValue();
        _delta = 0;
        _gateOn = false;
    }

    public override void GenerateMix()
    {
        var gate = Gate.GetValue();
        var stageChanged = false;

        if (gate > 0 && !_gateOn)
        {
            _gateOn = true;
            _stage = 0;
            _time = 0;
            _delta = 0;
            _level = _levels[0].GetValue();
            if (_stage != _releaseStage)
            {
                stageChanged = true;
            }
        }

        if (_gateOn && gate <= 0)
        {
            _gateOn = false;
            if (_releaseStage.HasValue)
            {
                _stage = _releaseStage;
                stageChanged = true;
            }
        }

        if (_changeTime.HasValue)
        {
            _time += 1;
            if (_time >= _changeTime)
            {
                _stage += 1;
                if (_stage != _releaseStage)
                {
                    stageChanged = true;
                }
                else
                {
                    _changeTime = null;
                    _delta = 0;
                }
            }
        }

        if (stageChanged)
        {
            if (_stage != _times.Length)
            {
                if (_stage != null)
                {
                    _delta = CalculateDelta(_stage.Value, _level);
                    if (_time != null) _changeTime = CalculateChangeTime(_stage.Value, _time.Value);
                }
            }
            else
            {
                OnComplete();
                _stage = null;
                _time = null;
                _changeTime = null;

                _delta = 0;
            }
        }

        _level += _delta;
        Outputs[0].Samples[0] = _level;
    }

    private double CalculateDelta(int newStage, double newLevel)
    {
        var newDelta = _levels[newStage + 1].GetValue() - newLevel;
        var stageTime = _times[newStage].GetValue() *  AudioProvider.SampleRate;
        return (newDelta / stageTime);
    }

    private int CalculateChangeTime(int stage, int time)
    {
        var stageTime = _times[stage].GetValue() * AudioProvider.SampleRate;
        return (int)(time + stageTime);
    }

    protected virtual void OnComplete()
    {
        Complete?.Invoke(this, EventArgs.Empty);
    }
}
