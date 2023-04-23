namespace SynthEngine;

public class BitCrusher : Effect
{
    private int _sampleRate;
    private double _resolution;
    private float _sample;

    public BitCrusher(int sampleRate = 44100, int bits = 8)
    {
        _sampleRate = sampleRate;
        _resolution = bits != 0 ? Math.Pow(2, bits - 1) : Math.Pow(2, 8 - 1);
    }

    public override void Generate()
    {
        throw new NotImplementedException();
    }

    public override float GetChannel(int channel)
    {
        return _sample;
    }

    public override void Reset()
    {
        throw new NotImplementedException();
    }

    public override float PushSample(float sample)
    {
        _sample =  (float) (Math.Floor(sample * _resolution + 0.5) / _resolution);
        return _sample;
    }
}
