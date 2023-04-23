using SynthesizerEngine.Core.Audio;

namespace SynthesizerEngine.DSP;

public class LowPassFilter : BiquadFilter
{
    public LowPassFilter(Provider provider, double frequency = 22100)
        : base(provider, frequency)
    {
    }

    protected override void CalculateCoefficients(float frequency)
    {
        var w0 = 2 * Math.PI * frequency / AudioProvider.WaveFormat.SampleRate;
        var cosW0 = Math.Cos(w0);
        var sinW0 = Math.Sin(w0);
        var alpha = sinW0 / (2 / Math.Sqrt(2));

        B0 = (1 - cosW0) / 2;
        B1 = 1 - cosW0;
        B2 = B0;
        A0 = 1 + alpha;
        A1 = -2 * cosW0;
        A2 = 1 - alpha;
    }

    public override string ToString()
    {
        return "Low Pass Filter";
    }
}
