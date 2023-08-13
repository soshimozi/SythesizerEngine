using SynthesizerEngine.Core.Audio.Interface;
using SynthesizerEngine.Scale;
using SynthesizerEngine.Tuning;

namespace SynthesizerEngine.Scale;

public class Scale : IScale
{
    private readonly List<int> _degrees;
    private readonly TuningBase _tuning;

    public Scale(List<int> degrees, TuningBase? tuning = null)
    {
        _tuning = tuning ?? new EqualTemperamentTuning(12);
        _degrees = degrees;
    }

    public double GetFrequency(int degree, double rootFrequency, int octave)
    {
        var frequency = rootFrequency;
        octave += (int)Math.Floor((double)degree / _degrees.Count);
        degree %= _degrees.Count;
        frequency *= Math.Pow(_tuning.OctaveRatio, octave);
        frequency *= _tuning.Ratios[_degrees[degree]];
        return frequency;
    }
}
