using SynthesizerEngine.Core.Audio;

namespace SynthesizerEngine.DSP;

public class PercussiveEnvelope : Envelope
{
    public PercussiveEnvelope(Provider provider, double gate, double attack, double release)
        : base(provider, gate, new[] { 0d, 1d, 0d }, new[] { attack, release })
    {
    }

    public override string ToString()
    {
        return "Percussive Envelope";
    }
}
