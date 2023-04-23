using SynthesizerEngine.DSP;

namespace SynthesizerEngine.Core.Audio;

public class Sink : Group
{
    private readonly UpMixer _mixer;
    public Sink(Provider provider, Device device, Scheduler scheduler) : base(provider, 1, 0)
    {
        _mixer = new UpMixer(provider, provider.WaveFormat.Channels);

        InputPassThroughNodes[0].Connect(scheduler);
        scheduler.Connect(_mixer);
        _mixer.Connect(device);
    }

    public override string ToString()
    {
        return "Destination";
    }

}
