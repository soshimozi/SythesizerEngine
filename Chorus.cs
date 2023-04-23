namespace SynthEngine;

using System;

public class Chorus : Effect
{
    public float DelayTime { get; set; }
    public uint Depth { get; set; }
    public float Freq { get; set; }
    public int SampleRate { get; set; }
    private Oscillator osc;
    private float[] buffer;
    private int bufferPos;
    private float sample;

    public Chorus(int sampleRate = 44100, float delayTime = 30, uint depth = 3, float freq = 0.1f)
    {
        DelayTime = delayTime;
        Depth = depth;
        Freq = freq;
        SampleRate = sampleRate;

        osc = new Oscillator(sampleRate, freq);
        CalcCoeff();
    }

    private void CalcCoeff()
    {
        buffer = new float[(int)(SampleRate * 0.1)];
        bufferPos = 0;

        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = 0.0f;
        }
    }

    public override float PushSample(float s)
    {
        if (++bufferPos >= buffer.Length)
        {
            bufferPos = 0;
        }

        buffer[bufferPos] = s;
        osc.Generate();

        float delay = DelayTime + osc.GetChannel(1) * Depth;
        delay *= SampleRate / 1000;
        delay = bufferPos - (float)Math.Floor(delay);

        while (delay < 0)
        {
            delay += buffer.Length;
        }

        if ((int)delay >= buffer.Length)
            delay = buffer.Length - 1;

        sample = buffer[(int)delay];
        return sample;
    }

    public override float GetChannel(int channel)
    {
        return sample;
    }

    public override void Generate()
    {
        throw new NotImplementedException();
    }

    public override void Reset()
    {
        throw new NotImplementedException();
    }
}
