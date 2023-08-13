using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class Envelope : AudioNode
{
    public Automation Gate { get; }

    protected readonly Automation[] Levels;
    protected readonly Automation[] Times;

    private readonly int? _releaseStage;

    private int? _stage;
    private int _time;
    private int? _changeTime;

    private double _level;
    private double _delta;
    private bool _gateOn;

    public event EventHandler<EventArgs>? Complete;

    protected Envelope(IAudioProvider provider, double? gate, IReadOnlyList<double> levels, IReadOnlyList<double> times, int? releaseStage = null)
        : base(provider, 1, 1)
    {
        Gate = new Automation(this, 0, gate ?? 1);

        Levels = new Automation[levels.Count];
        for (var i = 0; i < levels.Count; i++)
        {
            Levels[i] = new Automation(this, null, levels[i]);
        }

        Times = new Automation[times.Count];
        for (var i = 0; i < times.Count; i++)
        {
            Times[i] = new Automation(this, null, times[i]);
        }

        _releaseStage = releaseStage;

        _stage = null;
        _time = 0;
        _changeTime = null;

        _level = Levels[0];
        _delta = 0;
        _gateOn = false;
    }

    protected override void GenerateMix()
    {
        //var gate = Gate.GetValue();
        //var stageChanged = false;

        //if (gate > 0 && !_gateOn)
        //{
        //    _gateOn = true;
        //    _stage = 0;
        //    _time = 0;
        //    _delta = 0;
        //    _level = Levels[0].GetValue();
        //    if (_stage != _releaseStage)
        //    {
        //        stageChanged = true;
        //    }
        //}

        //if (_gateOn && gate <= 0)
        //{
        //    _gateOn = false;
        //    if (_releaseStage.HasValue)
        //    {
        //        _stage = _releaseStage;
        //        stageChanged = true;
        //    }
        //}

        //if (_changeTime.HasValue)
        //{
        //    _time += 1;
        //    if (_time >= _changeTime)
        //    {
        //        _stage += 1;
        //        if (_stage != _releaseStage)
        //        {
        //            stageChanged = true;
        //        }
        //        else
        //        {
        //            _changeTime = null;
        //            _delta = 0;
        //        }
        //    }
        //}

        //if (stageChanged)
        //{
        //    if (_stage != Times.Length)
        //    {
        //        if (_stage != null)
        //        {
        //            _delta = CalculateDelta(_stage.Value, _level);
        //            if (_time != null) _changeTime = CalculateChangeTime(_stage.Value, _time.Value);
        //        }
        //    }
        //    else
        //    {
        //        OnComplete();
        //        //_stage = null;
        //        //_time = null;
        //        _changeTime = null;

        //        _delta = 0;
        //    }
        //}

        //_level += _delta;
        //Outputs[0].Samples[0] = _level;

        double gate = Gate;
        var stageChanged = false;

        if (gate > 0 && !_gateOn)
        {
            _gateOn = true;
            _stage = 0;
            _time = 0;
            _delta = 0;
            _level = Levels[0];
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
                _stage = _releaseStage.Value;
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
            if (_stage < Times.Length)
            {
                if (Times[_stage.Value] != 0)
                {
                    _delta = CalculateDelta(_stage.Value, _level);
                    _changeTime = CalculateChangeTime(_stage.Value, _time);
                }
                else
                {
                    _level = Levels[_stage.Value + 1];
                    // Ensure we move to the next stage even if the current stage duration is 0
                    _stage += 1;
                    if (_stage < Times.Length)
                    {
                        // Ensure that we move to the Sustain stage when Decay is 0
                        if (_stage == 1 && Times[_stage.Value] == 0)
                        {
                            _stage += 1;
                            _level = Levels[_stage.Value];
                        }

                        _delta = CalculateDelta(_stage.Value, _level);
                        _changeTime = CalculateChangeTime(_stage.Value, _time);
                    }
                    else
                    {
                        OnComplete();
                        _changeTime = null;
                        _delta = 0;
                    }
                }
            }
            else
            {
                OnComplete();
                _changeTime = null;
                _delta = 0;
            }
        }


        _level += _delta;
        Outputs[0].Samples[0] = _level;
    }

    private double CalculateDelta(int newStage, double newLevel)
    {
        var newDelta = Levels[newStage + 1] - newLevel;
        var stageTime = Times[newStage] *  AudioProvider.SampleRate;

        if (stageTime != 0)
        {
            return (newDelta / stageTime);
        }
        else
        {
            return 0;
        }
    }

    private int CalculateChangeTime(int stage, int time)
    {
        var stageTime = Times[stage] * AudioProvider.SampleRate;
        return (int)(time + stageTime);
    }

    protected virtual void OnComplete()
    {
        Complete?.Invoke(this, EventArgs.Empty);
    }
}
