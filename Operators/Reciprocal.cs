using SynthesizerEngine.Core.Audio;

namespace SynthesizerEngine.Operators;

public class Reciprocal : Node
{

    public Reciprocal(Provider provider) : base(provider, 1, 1)
    {
        LinkNumberOfOutputChannels(0, 0);
    }

    protected override void GenerateMix()
    {
        var input = Inputs[0];
        var numberOfChannels = input.Samples.Count;

        for (var i = 0; i < numberOfChannels; i++)
        {
            Outputs[0].Samples[i] = 1 / input.Samples[i];
        }
    }

}