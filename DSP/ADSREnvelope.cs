using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class ADSREnvelope : Envelope
{
    //public double Attack => Times[0].GetValue();

    //public double Decay => Times[1].GetValue();

    //public double Sustain => Levels[2].GetValue();

    //public double Release => Times[2].GetValue();

    public ADSREnvelope(IAudioProvider provider, double gate, double attack, double decay, double sustain, double release)
        : base(provider, gate, new[] { 0, 1, sustain, 0 }, new [] { attack, decay, release }, 2)
    {
    }

}