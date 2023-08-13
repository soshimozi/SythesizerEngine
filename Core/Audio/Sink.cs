using SynthesizerEngine.Core.Audio.Interface;
using SynthesizerEngine.DSP;

namespace SynthesizerEngine.Core.Audio;

public class Sink : AudioNode
{
    public Sink(IAudioProvider provider, IAudioNode device, IAudioNode scheduler) : base(provider, 1, 0, true)
    {
        var mixer = new UpMixer(provider);

        InputPassThroughNodes[0].Connect(scheduler, 0, 0);

        scheduler.Connect(mixer, 0, 0);
        mixer.Connect(device);
    }

    public override string ToString()
    {
        return "Destination";
    }

}
