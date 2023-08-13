using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.Operators;

public class Tanh : AudioNode
{

    public Tanh(IAudioProvider provider) : base(provider, 1, 1)
    {
        LinkNumberOfOutputChannels(0, 0);
    }

    protected override void GenerateMix()
    {
        var input = Inputs[0];
        var numberOfChannels = input.Samples.Count;

        for (var i = 0; i < numberOfChannels; i++)
        {
            var value = input.Samples[i];

            Outputs[0].Samples[i] = (Math.Exp(value) - Math.Exp(-value)) /
                                    (Math.Exp(value) + Math.Exp(-value));
        }
    }

}