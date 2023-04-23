using NAudio.Wave;
using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;

namespace SynthesizerEngine.DSP;

public class Compressor : Node
{
    private readonly Automation _scaleBy;
    private readonly Automation _gain;

    public Compressor(Provider provider, double gain = 0.5, uint scaleBy = 1) : base(provider, 3, 1)
    {
        LinkNumberOfOutputChannels(0, 0);

        _gain = new Automation(this, 1, gain);
        _scaleBy = new Automation(this, 2, scaleBy);
    }

    protected override void GenerateMix()
    {
        var input = Inputs[0];
        var output = Outputs[0];

        var gain = _gain.GetValue();
        var scale = _scaleBy.GetValue();

        var numberOfChannels = input.Samples.Count;
        for (var i = 0; i < numberOfChannels; i++)
        {
            var s = input.Samples[i];

            s /= scale;
            var sample = (1 + gain) * s - gain * s * s * s;

            output.Samples[i] = sample;
        }
    }
}