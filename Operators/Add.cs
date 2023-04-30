using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.Operators;

public class Add : Node
{
    protected readonly Automation Value;

    public Add(IAudioProvider provider, double? value = null) : base(provider, 2, 1)
    {
        LinkNumberOfOutputChannels(0, 0);
        Value = new Automation(this, 1, value.GetValueOrDefault(0));
    }

    public override void GenerateMix()
    {
        var value = Value.GetValue();
        var input = Inputs[0];
        var numberOfChannels = input.Samples.Count;

        for (var i = 0; i < numberOfChannels; i++)
        {
            Outputs[0].Samples[i] = input.Samples[i] + value;
        }
    }
}