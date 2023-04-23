using SynthesizerEngine.Core.Audio;

namespace SynthesizerEngine.Core;

public class Automation
{
    private Node _node;
    private readonly InputChannel? _input;
    private double _value;

    public Automation(Node node, int? inputIndex = null, double value = 0)
    {
        _node = node;
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
