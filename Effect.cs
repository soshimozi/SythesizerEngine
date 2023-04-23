using NAudio.Wave;

namespace SynthEngine;

public abstract class Effect : Generator
{
    //public double Mix { get; set; }
    //public int ChannelCount { get; set; }
    protected Dictionary<string, object> Params { get; set; }

    protected Effect()
    {
        Params = new Dictionary<string, object>();
    }

    public void SetParam(string param, object value)
    {
        Params[param] = value;
    }

    public abstract float PushSample(float sample);

    public override int Read(float[] buffer, int offset, int sampleCount)
    {
        for (var i = 0; i < sampleCount; i += WaveFormat.Channels)
        {
            for (var n = 0; n < WaveFormat.Channels; n++)
            {
                PushSample(buffer[i + n]);
                buffer[i + offset + n] = GetChannel(n) * Mix + buffer[i + offset + n] * (1 - Mix);
            }
        }

        return sampleCount;
    }
}
