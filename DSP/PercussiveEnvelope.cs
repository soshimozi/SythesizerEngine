using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class PercussiveEnvelope : Envelope
{
    public PercussiveEnvelope(IAudioProvider provider, double gate, double attack, double release)
        : base(provider, gate, new[] { 0d, 1d, 0d }, new[] { attack, release })
    {
    }

    public override string ToString()
    {
        return "Percussive Envelope";
    }
}
