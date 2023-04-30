using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class HighPassFilter : BiquadFilter
{
    protected HighPassFilter(IAudioProvider provider, double frequency) : base(provider, frequency)
    {
    }

    protected override void CalculateCoefficients(float frequency)
    {
        var w0 = 2 * Math.PI * frequency /
                 AudioProvider.SampleRate;
        var cosw0 = Math.Cos(w0);
        var sinw0 = Math.Sin(w0);
        var alpha = sinw0 / (2 / Math.Sqrt(2));

        B0 = (1 + cosw0) / 2;
        B1 = -(1 + cosw0);
        B2 = B0;
        A0 = 1 + alpha;
        A1 = -2 * cosw0;
        A2 = 1 - alpha;
    }
}