namespace SynthesizerEngine.Tuning;

public class Tuning
{
    private readonly List<double> _semitones;
    public double OctaveRatio { get;  }
    public List<double> Ratios { get; }

    protected Tuning(List<double> semitones, double octaveRatio = 2)
    {
        _semitones = semitones;
        OctaveRatio = octaveRatio;
        Ratios = new List<double>();
        var tuningLength = _semitones.Count;
        for (var i = 0; i < tuningLength; i++)
        {
            Ratios.Add(Math.Pow(2, _semitones[i] / tuningLength));
        }
    }
}