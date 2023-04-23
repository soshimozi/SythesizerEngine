using SynthesizerEngine.Scale;
using SynthesizerEngine.Tuning;

namespace SynthesizerEngine.Scale;

public class Scale
{
    private readonly List<int> _degrees;
    private readonly Tuning.Tuning _tuning;

    public Scale(List<int> degrees, Tuning.Tuning? tuning = null)
    {
        if (tuning == null)
        {
            _tuning =  new EqualTemperamentTuning(12);
        }
        else
        {
            _tuning = tuning;
        }

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
