using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthEngine;

using NAudio.Wave;
using System;

public abstract class Generator : WaveProvider32
{
    public float Mix { get; set; }
    public float[] GeneratedBuffer { get; protected set; }
    public int ChannelCount { get; protected set; }

    protected Generator()
    {
        Mix = 1;
        GeneratedBuffer = null;
        ChannelCount = 1;
    }

    public override int Read(float[] buffer, int offset, int sampleCount)
    {
        for (var index = 0; index < sampleCount; index += WaveFormat.Channels)
        {
            Generate();

            //buffer[offset + index] = GetMix(0) * Mix;
            for (var n = 0; n < WaveFormat.Channels; ++n)
            {
                buffer[offset + index + n] = GetChannel(n) * Mix;
            }
        }

        return sampleCount;
    }

    public abstract void Generate();

    public abstract float GetChannel(int channel);

    public abstract void Reset();
}

