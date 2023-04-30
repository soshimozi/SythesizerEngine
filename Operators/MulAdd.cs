using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.Operators;

public class MulAdd : Node
{
    private readonly Automation _mul;
    private readonly Automation _add;

    public MulAdd(IAudioProvider provider, double? mul = null, double? add = null) : base(provider, 3, 1)
    {
        LinkNumberOfOutputChannels(0, 0);
        _mul = new Automation(this, 1, mul.GetValueOrDefault(1));
        _add = new Automation(this, 2, add.GetValueOrDefault(0));
    }

    public override void GenerateMix()
    {
        var input = Inputs[0];
        var output = Outputs[0];

        var mul = _mul.GetValue();
        var add = _add.GetValue();

        var numberOfChannels = input.Samples.Count;
        for (var i = 0; i < numberOfChannels; i++)
        {
            output.Samples[i] = input.Samples[i] * mul + add;
        }
    }

}
