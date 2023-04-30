using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.Core;

public class Automation
{
    private readonly IChannel? _input;
    private double _value;

    public Automation(IAudioNode node, int? inputIndex = null, double value = 0)
    {
        _input = inputIndex.HasValue ? node.Inputs[inputIndex.Value] : null;
        _value = value;
    }

    public bool IsStatic()
    {
        return (_input?.Samples.Count == 0);
    }

    public bool IsDynamic()
    {
        return (_input?.Samples.Count > 0);
    }

    public void SetValue(double value)
    {
        _value = value;
    }

    public double GetValue()
    {
        if (_input != null && _input.Samples.Count > 0)
        {
            return _input.Samples[0];
        }
            
        return _value;
    }
}
