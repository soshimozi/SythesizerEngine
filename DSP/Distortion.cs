using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class Distortion : Node
{
    private readonly Automation _overdrive;
    private readonly Automation _masterGain;

    private readonly IIRFilter[] _filters = new IIRFilter[3];

    private double _smpl;

    public Distortion(IAudioProvider provider, double overdrive = 4, double masterGain = 1.0) : base(provider, 3, 1)
    {
        LinkNumberOfOutputChannels(0, 0);

        _overdrive = new Automation(this, 1, overdrive);
        _masterGain = new Automation(this, 2, masterGain);

        var hpf1 = new IIRFilter(provider.SampleRate, 720.484);
        var lpf1 = new IIRFilter(provider.SampleRate, 723.431);
        var hpf2 = new IIRFilter(provider.SampleRate, 1.0);


        _filters[0] = hpf1;
        _filters[1] = lpf1;
        _filters[2] = hpf2;

    }

    public override void GenerateMix()
    {
        
        var input = Inputs[0];
        var output = Outputs[0];

        var hpf1 = _filters[0];
        var lpf1 = _filters[1];
        var hpf2 = _filters[2];

        var numberOfChannels = input.Samples.Count;

        for (var i = 0; i < numberOfChannels; i++)
        {
            var smpl = input.Samples[i];

            hpf1.PushSample(smpl);
            smpl = hpf1.GetMix(1) * _overdrive.GetValue();
            smpl = Math.Atan(smpl) + smpl;
            
            if (smpl > 0.4)
            {
                smpl = 0.4;
            }
            else if (smpl < -0.4)
            {
                smpl = -0.4;
            }

            lpf1.PushSample(smpl);
            hpf2.PushSample(lpf1.GetMix(0));
            smpl = hpf2.GetMix(1) * _masterGain.GetValue();

            output.Samples[i] = smpl;
        }

    }
}