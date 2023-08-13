using SynthesizerEngine.Core.Audio.Interface;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core;

namespace SynthesizerEngine.Operators;

public class Constant : AudioNode
{
    private readonly Automation _value;

    public Constant(IAudioProvider provider, double? value = null) : base(provider, 1, 1)
    {
        LinkNumberOfOutputChannels(0, 0);
        _value = new Automation(this, 0, value.GetValueOrDefault(1));
    }

    protected override void GenerateMix()
    {
        var value = _value.GetValue();
        var numberOfChannels = AudioProvider.Channels;

        for (var i = 0; i < numberOfChannels; i++)
        {
            // output a constant value
            Outputs[0].Samples[i] = value;
        }
    }
}