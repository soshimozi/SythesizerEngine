using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class BiquadFilter : AudioNode
{
    private readonly Automation _frequency;

    private double _lastFrequency;

    // Delayed values
    private readonly List<double[]> _xValues;
    private readonly List<double[]> _yValues;

    // Coefficients
    protected double B0;
    protected double B1;
    protected double B2;
    protected double A0;
    protected double A1;
    protected double A2;

    protected BiquadFilter(IAudioProvider provider, double frequency = 22100)
        : base(provider, 2, 1)
    {
        LinkNumberOfOutputChannels(0, 0);

        _frequency = new Automation(this, 1, frequency);
        _lastFrequency = 0;

        _xValues = new List<double[]>();
        _yValues = new List<double[]>();

        B0 = 0;
        B1 = 0;
        B2 = 0;
        A0 = 0;
        A1 = 0;
        A2 = 0;
    }

    protected virtual void CalculateCoefficients(float frequency)
    {
        // This method should be overridden in derived classes
    }

    protected override void GenerateMix()
    {
        var input = Inputs[0];
        var output = Outputs[0];
        var xValueArray = _xValues;
        var yValueArray = _yValues;

        var frequency = (float)(double) _frequency;

        // frequency != _lastFrequency
        if (Math.Abs(frequency - _lastFrequency) > double.Epsilon)
        {
            CalculateCoefficients(frequency);
            _lastFrequency = frequency;
        }

        var a0 = A0;
        var a1 = A1;
        var a2 = A2;
        var b0 = B0;
        var b1 = B1;
        var b2 = B2;

        var numberOfChannels = input.Samples.Count;
        for (var i = 0; i < numberOfChannels; i++)
        {
            if (i >= xValueArray.Count)
            {
                xValueArray.Add(new double[] { 0, 0 });
                yValueArray.Add(new double[] { 0, 0 });
            }

            var xValues = xValueArray[i];
            var x1 = xValues[0];
            var x2 = xValues[1];
            var yValues = yValueArray[i];
            var y1 = yValues[0];
            var y2 = yValues[1];

            var x0 = input.Samples[i];
            var y0 = (b0 / a0) * x0 +
                     (b1 / a0) * x1 +
                     (b2 / a0) * x2 -
                     (a1 / a0) * y1 -
                     (a2 / a0) * y2;

            output.Samples[i] = y0;

            xValues[0] = x0;
            xValues[1] = x1;
            yValues[0] = y0;
            yValues[1] = y1;
        }
    }

    public override string ToString()
    {
        return "Biquad Filter";
    }
}
