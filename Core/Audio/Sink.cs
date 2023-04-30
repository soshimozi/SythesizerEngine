using SynthesizerEngine.Core.Audio.Interface;
using SynthesizerEngine.DSP;

namespace SynthesizerEngine.Core.Audio;

public class Sink : GroupNode
{
    private readonly UpMixer _mixer;
    public Sink(IAudioProvider provider, Device device, Scheduler scheduler) : base(provider, 1, 0)
    {
        _mixer = new UpMixer(provider);

        (InputPassThroughNodes[0] as PassThroughNode)?.Connect(scheduler);

        scheduler.Connect(_mixer);
        _mixer.Connect(device);
    }

    public override string ToString()
    {
        return "Destination";
    }

}
