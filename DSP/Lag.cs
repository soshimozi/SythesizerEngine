using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

/// <summary>
/// Exponential lag for smoothing signals.
///
/// *** Inputs**
///
/// - Value
/// - Lag time
///
/// *** Outputs**
///
/// - Lagged value
///
/// *** Parameters**
///
/// - value The value to lag.Linked to input 0.
/// - lag The 60dB lag time.Linked to input 1.
///
/// </summary>
public class Lag : AudioNode
{
    private readonly Automation _value;
    private readonly Automation _lag;

    private double _lastValue = 0;
    private readonly double log001;

    public Lag(IAudioProvider provider, double value = 0, double lagTime = 1) : base(provider, 2, 1)
    {
        LinkNumberOfOutputChannels(0, 0);

        _value = new Automation(this, 0, value);
        _lag = new Automation(this, 1, lagTime);

        log001 = Math.Log(0.001);

    }

    protected override void GenerateMix()
    {
        var input = Inputs[0];
        var output = Outputs[0];


        var value = _value.GetValue();
        var lag = _lag.GetValue();
        var coefficient = Math.Exp(log001 / (lag * AudioProvider.SampleRate));

        var outputValue = ((1 - coefficient) * value) +
                          (coefficient * _lastValue);
        output.Samples[0] = outputValue;
        _lastValue = outputValue;
    }
}