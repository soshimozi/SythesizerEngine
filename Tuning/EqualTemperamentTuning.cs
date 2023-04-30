namespace SynthesizerEngine.Tuning;

public class EqualTemperamentTuning : TuningBase
{
    public EqualTemperamentTuning(int pitchesPerOctave) : base(InitializeSemiToneList(pitchesPerOctave))
    {
    }

    private static List<double> InitializeSemiToneList(int pitchesPerOctave)
    {
        var list = new List<double>();
        for (var i = 0; i < pitchesPerOctave; i++)
        {
            list.Add(i);
        }

        return list;
    }
}
