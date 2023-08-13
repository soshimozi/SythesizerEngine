using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class SoftClip : AudioNode
{
    public SoftClip(IAudioProvider provider) : base(provider, 1, 1)
    {
        LinkNumberOfOutputChannels(0, 0);
    }

    protected override void GenerateMix()
    {
        var input = Inputs[0];
        var output = Outputs[0];

        var numberOfChannels = input.Samples.Count;
        for (var i = 0; i < numberOfChannels; i++)
        {
            var value = input.Samples[i];
            if (value is > 0.5 or < -0.5)
            {
                output.Samples[i] = (Math.Abs(value) - 0.25) / value;
            }
            else
            {
                output.Samples[i] = value;
            }
        }
    }

    public override string ToString()
    {
        return "SoftClip";
    }
}
