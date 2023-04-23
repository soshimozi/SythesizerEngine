using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;

namespace SynthesizerEngine.DSP;

public class Oscillator : Node
{
    private readonly Automation _frequency;
    private readonly Automation _pulseWidth;

    private double _phase;
    private double _p;

    private WaveShape _waveform;

    public Oscillator(Provider audioLib, float frequency = 440, WaveShape waveForm = WaveShape.Sine, float pulseWidth = 0.5f) : base(audioLib, 2, 1)
    {
        LinkNumberOfOutputChannels(0, 0);

        _frequency = new Automation(this, 0, frequency);
        _pulseWidth = new Automation(this, 1, pulseWidth);
        _waveform = waveForm;

        _phase = 0.0;
        _p = 0.0;
    }

    public void SetWaveform(WaveShape waveform)
    {
        _waveform = waveform;
    }

    protected override void GenerateMix()
    {
        var output = Outputs[0];

        var pulseWidth = _pulseWidth.GetValue();
        var frequency = _frequency.GetValue();

        output.Samples[0] = GetMix();

        _phase = (_phase + frequency / AudioProvider.WaveFormat.SampleRate / 2) % 1;
        var p = (_phase) % 1;
        _p = p < pulseWidth ? p / pulseWidth : (p - pulseWidth) / (1 - pulseWidth);
    }

    private double GetMix()
    {
        return _waveform switch
        {
            WaveShape.Sine => Sine(),
            WaveShape.Triangle => Triangle(),
            WaveShape.Square => Square(),
            WaveShape.Sawtooth => Sawtooth(),
            WaveShape.InvSawtooth => InvSawtooth(),
            WaveShape.Pulse => Pulse(),
            _ => throw new ArgumentOutOfRangeException(nameof(_waveform))
        };
    }

    public double GetPhase()
    {
        return _p;
    }

    public void Reset(double phase = 0.0)
    {
        _phase = _p = phase;
    }

    private double Sine()
    {
        return Math.Sin(_p * Math.PI * 2);
    }

    private double Triangle()
    {
        return _p < 0.5 ? 4 * _p - 1 : 3 - 4 * _p;
    }

    private double Square()
    {
        return _p < 0.5 ? -1 : 1;
    }

    private double Sawtooth()
    {
        return 1 - _p * 2;
    }

    private double InvSawtooth()
    {
        return _p * 2 - 1;
    }

    private double Pulse()
    {
        return _p < 0.5 ? (_p < 0.25 ? _p * 8 - 1 : 1 - (_p - 0.25) * 8) : -1;
    }

}