using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;

namespace SynthesizerEngine.DSP;

public class LinearCrossFade : Node
{
    private readonly Automation _position;
    public LinearCrossFade(Provider audioProvider, double position = 0.5) : base(audioProvider, 3, 1)
    {
        LinkNumberOfOutputChannels(0, 0);

        _position = new Automation(this, 2, 0.5);
    }

    protected override void GenerateMix()
    {
        var inputA = Inputs[0];
        var inputB = Inputs[1];
        var output = Outputs[0];

        var position = _position.GetValue();

        var gainA = 1 - position;
        var gainB = position;

        var numberOfChannels = output.Samples.Count;
        for (var i = 0; i < numberOfChannels; i++)
        {
            
            double valueA, valueB;
            if (i < inputA.Samples.Count)
            {
                valueA = inputA.Samples[i];
            }
            else
            {
                valueA = 0;
            }

            if (i < inputB.Samples.Count)
            {
                valueB = inputB.Samples[i];
            }
            else
            {
                valueB = 0;
            }

            output.Samples[i] = valueA * gainA + valueB * gainB;
        }
    }
}