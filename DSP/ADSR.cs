using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class ADSR : AudioNode, IEnvelope
{
    public double AttackTime
    {
        get => Attack;
        set => Attack.SetValue(value);
    }

    public double DecayTime { get => Decay; set => Decay.SetValue(value); }
    public double SustainLevel { get => Sustain; set => Sustain.SetValue(value); }
    public double ReleaseTime { get => Release; set => Release.SetValue(value); }

    protected readonly Automation Attack;
    protected readonly Automation Decay;
    protected readonly Automation Sustain;
    protected readonly Automation Release;

    private double _currentLevel = 0;
    private double _targetLevel = 0;
    private double _timeToReachTarget = 0;
    private double _timeElapsed = 0;

    public void NoteOn()
    {
        _targetLevel = 1.0;
        _timeToReachTarget = AttackTime;
        _timeElapsed = 0;
    }

    public void NoteOff()
    {
        _targetLevel = 0;
        _timeToReachTarget = ReleaseTime;
        _timeElapsed = 0;
    }

    public ADSR(IAudioProvider provider, double attack = 0, double decay = 0, double sustain = 0, double release = 0) : base(provider, 4, 1)
    {
        Attack = new Automation(this, 0, attack);
        Decay = new Automation(this, 1, decay);
        Sustain = new Automation(this, 2, sustain);
        Release = new Automation(this, 3, release);
    }

    protected override void GenerateMix()
    {
        if (_timeElapsed < _timeToReachTarget)
        {
            _currentLevel += (_targetLevel - _currentLevel) * (_timeElapsed / _timeToReachTarget);
            _timeElapsed += 1.0 / AudioProvider.SampleRate; // 1 / sample rate to convert to time
        }
        else if (Math.Abs(_targetLevel - 1.0) < double.Epsilon && _currentLevel >= _targetLevel)
        {
            _targetLevel = SustainLevel;
            _timeToReachTarget = DecayTime;
            _timeElapsed = 0;
        }

        Outputs[0].Samples[0] = _currentLevel;
    }
}