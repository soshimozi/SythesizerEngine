using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class TriggerControl : Node
{
    private readonly Automation _trigger;

    public TriggerControl(IAudioProvider provider, double trigger = 0) : base(provider, 0, 1)
    {
        _trigger = new Automation(this, null, trigger);
    }

    public override void GenerateMix()
    {
        if (_trigger.GetValue() > 0)
        {
            Outputs[0].Samples[0] = 1;
            _trigger.SetValue(0);
        }
        else
        {
            Outputs[0].Samples[0] = 0;
        }
    }

    public override string ToString()
    {
        return "Trigger Control";
    }
}
