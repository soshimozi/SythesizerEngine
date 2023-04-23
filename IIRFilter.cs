namespace SynthEngine;

public class IIRFilter : Effect
{
    private float _freq = 0;
    private float _damp = 0;
    private float _prevCut = 0;
    private float _prevReso = 0;

    private float _cutoff;
    private float _resonance;
    private float[] _f = new [] {0.0f, 0.0f, 0.0f, 0.0f};
    private int _sampleRate;
    private int _type;

    public IIRFilter(int sampleRate = 44100, float cutoff = 20000, float resonance = 0.1f, int type = 0)
    {
        _cutoff = cutoff;
        _resonance = resonance;
        _sampleRate = sampleRate;
        _type = type;

    }

    private void CalcCoeff()
    {
        _freq = (float)(2 * Math.Sin(Math.PI * Math.Min(0.25, _cutoff / (_sampleRate * 2))));
        _damp = (float)Math.Min(2 * (1 - Math.Pow(_resonance, 0.25)), Math.Min(2, 2 / _freq - _freq * 0.5));
    }
    public override void Generate()
    {
        throw new NotImplementedException();
    }

    public override float GetChannel(int channel)
    {
        throw new NotImplementedException();
    }

    public float GetMixForFilterType(int type)
    {
        return (float) _f[type];
    }

    public override void Reset()
    {
        throw new NotImplementedException();
    }

    public override float PushSample(float sample)
    {
        var f = _f;

        if (Math.Abs(_prevCut - _cutoff) > Double.Epsilon || Math.Abs(_prevReso - _resonance) > Double.Epsilon)
        {
            CalcCoeff();
            _prevCut = _cutoff;
            _prevReso = _resonance;
        }

        f[3] = sample - _damp * f[2];
        f[0] = f[0] + _freq * f[2];
        f[1] = f[3] - f[0];
        f[2] = _freq * f[1] + f[2];

        f[3] = sample - _damp * f[2];
        f[0] = f[0] + _freq * f[2];
        f[1] = f[3] - f[0];
        f[2] = _freq * f[1] + f[2];

        return f[_type];
    }
}