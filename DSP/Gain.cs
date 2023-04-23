﻿using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Operators;

namespace SynthesizerEngine.DSP;

public class Gain : Multiply
{
    private Automation _gainParameter;

    public Gain(Provider provider, double gain = 1) : base(provider, gain)
    {
        _gainParameter = Value;
    }

    public override string ToString()
    {
        return "Gain";
    }
}
