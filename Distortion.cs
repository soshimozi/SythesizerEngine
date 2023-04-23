namespace SynthEngine;

public class Distortion : Effect
{
    private float _sample = 0.0f;
    private readonly int _sampleRate;

    private readonly IIRFilter hpf1;
    private readonly IIRFilter lpf1;
    private readonly IIRFilter hpf2;
    private readonly IIRFilter[] _filters;

    public Distortion(int sampleRate)
    {
        _sampleRate = sampleRate;
        Gain = 4.0f;
        Master = 1.0f;

        this.hpf1 = new IIRFilter(sampleRate, 720.484f);
        this.lpf1 = new IIRFilter(sampleRate, 723.431f);
        this.hpf2 = new IIRFilter(sampleRate, 1.0f);

        _filters = new[] { this.hpf1, this.lpf1, this.hpf2 };
    }

    public float Gain { get; set; }
    public float Master { get; set; }

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
        this.hpf1.PushSample(sample);
        _sample = this.hpf1.GetMixForFilterType(1) * Gain;
        _sample = (float)Math.Atan(_sample) + _sample;
        if (_sample > 0.4)
        {
            _sample = 0.4f;
        }
        else if (_sample < -0.4)
        {
            _sample = -0.4f;
        }

        lpf1.PushSample(_sample);
        hpf2.PushSample(lpf1.GetMixForFilterType(0));
        _sample = this.hpf2.GetMixForFilterType(1) * Master;
        return _sample;
    }

}