using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class UpMixer : AudioNode
{
    public UpMixer(IAudioProvider provider) : base(provider, 1, 1)
    {
        Outputs[0].Channels = provider.Channels;
    }

    protected override void GenerateMix()
    {
        var input = Inputs[0];
        var output = Outputs[0];

        var numberOfInputChannels = input.Samples.Count;
        var numberOfOutputChannels = output.Samples.Count;

        if (numberOfInputChannels == numberOfOutputChannels)
        {
            output.Samples.Clear();
            output.Samples.AddRange(input.Samples.ToArray());
        }
        else
        {
            for (var i = 0; i < numberOfOutputChannels; i++)
            {
                if (numberOfInputChannels == 0)
                    output.Samples[i] = 0;
                else
                    output.Samples[i] = input.Samples[i % numberOfInputChannels];
            }
        }
    }
}
