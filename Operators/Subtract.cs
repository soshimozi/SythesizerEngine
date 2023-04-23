using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;

namespace SynthesizerEngine.Operators;

public class Subtract : Node
{
    private readonly Automation _value;

    public Subtract(Provider provider, double? value = null) : base(provider, 2, 1)
    {
        LinkNumberOfOutputChannels(0, 0);
        _value = new Automation(this, 1, value.GetValueOrDefault(0));
    }

    protected override void GenerateMix()
    {
        var value = _value.GetValue();
        var input = Inputs[0];
        var numberOfChannels = input.Samples.Count;

        for (var i = 0; i < numberOfChannels; i++)
        {
            Outputs[0].Samples[i] = input.Samples[i] - value;
        }
    }
}