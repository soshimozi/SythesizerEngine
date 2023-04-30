using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class BitCrusher : Node
{
    private readonly Automation _bits;
    public BitCrusher(IAudioProvider provider, int bits = 8) : base(provider, 2, 1)
    {
        LinkNumberOfOutputChannels(0, 0);
        _bits = new Automation(this, 1, bits);
    }

    public override void GenerateMix()
    {
        var input = Inputs[0];

        var maxValue = Math.Pow(2, _bits.GetValue()) - 1;

        var numberOfChannels = input.Samples.Count;
        for (var i = 0; i < numberOfChannels; i++)
        {
            Outputs[0].Samples[i] = Math.Floor(input.Samples[i] * maxValue) /
                                         maxValue;
        }
    }
}