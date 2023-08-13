using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class ADSREnvelope : Envelope
{
    public double Attack
    {
        get => Times[0];
        set => Times[0].SetValue(value);
    }

    public double Decay
    {
        get => Times[1];
        set => Times[1].SetValue(value);
    }

    public double Release
    {
        get => Times[2];
        set => Times[2].SetValue(value);
    }

    public double Sustain
    {
        get => Levels[2];
        set => Levels[2].SetValue(value);
    }

    public ADSREnvelope(IAudioProvider provider, double gate, double attack, double decay, double sustain, double release)
        : base(provider, gate, new[] { 0, 1, sustain, 0 }, new [] { attack, decay, release }, 2)
    {
    }

}