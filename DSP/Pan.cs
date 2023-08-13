using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class Pan : AudioNode
{
    private readonly Automation _pan;

    protected Pan(IAudioProvider audioLib, double pan = 0.5) : base(audioLib, 2, 1)
    {
        // Hardcode two output channels
        SetNumberOfOutputChannels(0, 2);
        _pan = new Automation(this, 1, pan);
    }

    protected override void GenerateMix()
    {
        var input = Inputs[0];
        var output = Outputs[0];

        var pan = _pan.GetValue();

        var value = input.Samples[0];
        var scaledPan = pan * Math.PI / 2;
        output.Samples[0] = value * Math.Cos(scaledPan);
        output.Samples[1] = value * Math.Sin(scaledPan);
    }
}