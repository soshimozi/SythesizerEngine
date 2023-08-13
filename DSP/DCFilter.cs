using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class DCFilter : AudioNode
{
    private readonly Automation _coefficient;

    // Delayed values
    private readonly List<double> _xValues = new List<double>();
    private readonly List<double> _yValues = new List<double>();
    public DCFilter(IAudioProvider provider, double coefficient = 0.995) : base(provider, 2, 1)
    {
        // Same number of output channels as input channels
        LinkNumberOfOutputChannels(0, 0);

        _coefficient = new Automation(this, 1, coefficient);
    }

    protected override void GenerateMix()
    {
        var coefficient = _coefficient.GetValue();
        var input = Inputs[0];
        var numberOfChannels = input.Samples.Count;
        for (var i = 0; i < numberOfChannels; i++)
        {
            if (i >= _xValues.Count)
            {
                _xValues.Add(0);
            }

            if (i >= _yValues.Count)
            {
                _yValues.Add(0);
            }

            var x0 = input.Samples[i];
            var y0 = x0 - _xValues[i] + coefficient * _yValues[i];

            Outputs[0].Samples[i] = y0;

            _xValues[i] = x0;
            _yValues[i] = y0;
        }
    }
}