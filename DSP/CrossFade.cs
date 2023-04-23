using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;

namespace SynthesizerEngine.DSP;

public class CrossFade : Node
{
    private readonly Automation _position;
    public CrossFade(Provider provider, double position = 0.5) : base(provider, 3, 1)
    {
        LinkNumberOfOutputChannels(0, 0);
        _position = new Automation(this, 2, position);
    }

    protected override void GenerateMix()
    {
        var inputA = Inputs[0];
        var inputB = Inputs[1];
        var output = Outputs[0];

        // Local processing variables
        var position = _position.GetValue();

        var scaledPosition = position * Math.PI / 2;
        var gainA = Math.Cos(scaledPosition);
        var gainB = Math.Sin(scaledPosition);

        var numberOfChannels = output.Samples.Count;
        for (var i = 0; i < numberOfChannels; i++)
        {
            double valueA = 0, valueB = 0;
            if(i < inputA.Samples.Count)
                valueA = inputA.Samples[i];

            if (i < inputA.Samples.Count)
                valueB = inputB.Samples[i];

            output.Samples[i] = valueA * gainA + valueB * gainB;
        }
    }
}