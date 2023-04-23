using SynthesizerEngine.Core;

namespace SynthesizerEngine.DSP;


public class IIRFilter
{
    private double[] f = new double[4];
    private double freq, damp;
    private double prevCut, prevReso;

    public double Cutoff { get; set; }
    public double Resonance { get; set; }
    public int SampleRate { get; set; }
    public int Type { get; set; }

    public IIRFilter(int sampleRate, double cutoff = 20000, double resonance = 0.1, int type = 0)
    {
        Cutoff = cutoff;
        Resonance = resonance;
        SampleRate = sampleRate;
        Type = type;

        prevCut = 0;
        prevReso = 0;
    }

    private void CalcCoeff()
    {
        freq = 2 * Math.Sin(Math.PI * Math.Min(0.25, Cutoff / (SampleRate * 2)));
        damp = Math.Min(2 * (1 - Math.Pow(Resonance, 0.25)), Math.Min(2, 2 / freq - freq * 0.5));
    }

    public double PushSample(double sample)
    {
        if (prevCut.CompareTo(Cutoff) != 0 || prevReso.CompareTo(Resonance) != 0) 
        {
            CalcCoeff();
            prevCut = Cutoff;
            prevReso = Resonance;
        }

        f[3] = sample - damp * f[2];
        f[0] = f[0] + freq * f[2];
        f[1] = f[3] - f[0];
        f[2] = freq * f[1] + f[2];

        f[3] = sample - damp * f[2];
        f[0] = f[0] + freq * f[2];
        f[1] = f[3] - f[0];
        f[2] = freq * f[1] + f[2];

        return f[Type];
    }

    public double GetMix(int? type = null)
    {
        return type == null ? f[Type] : f[type.Value];
    }
    //private readonly double[] _f = { 0.0, 0.0, 0.0, 0.0};

    //   private double _previousCutoff;
    //   private double _previousResonance;

    //   public double  Cutoff { get; set; }
    //   public double Resonance { get; set; }

    //   public int FilterType { get; set; }

    //   private double _freq = 0, _damp = 0;

    //   private double _sampleRate;
    //   public IIRFilter(double sampleRate = 44100, double cutoff = 20000, double resonance = 0.1, int type = 0)
    //   {

    //       Cutoff = cutoff;
    //       Resonance = resonance;

    //       _sampleRate = sampleRate;

    //       FilterType = type;

    //       CalcCoefficients();
    //       _previousCutoff = cutoff;
    //       _previousResonance = resonance;
    //   }

    //   private void CalcCoefficients()
    //   {
    //       _freq = 2 * Math.Sin(Math.PI * Math.Min(0.25, Cutoff / (_sampleRate * 2)));
    //       _damp = Math.Min(2 * (1 - Math.Pow(Cutoff, 0.25)), Math.Min(2, 2 / _freq - _freq * 0.5));
    //   }

    //   public double GenerateMix(double sample)
    //   {
    //       if ((_previousCutoff.CompareTo(Cutoff) != 0) ||
    //           (_previousResonance.CompareTo(Resonance) != 0))
    //       {
    //           CalcCoefficients();
    //           _previousCutoff = Cutoff;
    //           _previousResonance = Resonance;
    //       }


    //       _f[3] = sample - _damp * _f[2];
    //       _f[0] = _f[0] + _freq * _f[2];
    //       _f[1] = _f[3] - _f[0];
    //       _f[2] = _freq * _f[1] + _f[2];

    //       _f[3] = sample - _damp * _f[2];
    //       _f[0] = _f[0] + _freq * _f[2];
    //       _f[1] = _f[3] - _f[0];
    //       _f[2] = _freq * _f[1] + _f[2];

    //       return _f[FilterType];
    //   }
}