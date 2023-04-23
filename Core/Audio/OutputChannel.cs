namespace SynthesizerEngine.Core.Audio;


public class OutputChannel
{
    public Node Node { get; }
    public int Index { get; }
    public List<InputChannel> ConnectedTo { get; }
    public List<double> Samples { get; set; }

    private InputChannel? _linkedInput;

    public int NumberChannels
    {
        set => _numberChannels = value;
    }

    private int _numberChannels;

    public OutputChannel(Node node, int index)
    {
        Node = node;
        Index = index;
        ConnectedTo = new List<InputChannel>();
        Samples = new List<double>();

        _linkedInput = null;
        _numberChannels = 1;
    }

    public void Connect(InputChannel input)
    {
        ConnectedTo.Add(input);
    }

    public void Disconnect(InputChannel input)
    {
        ConnectedTo.Remove(input);
    }

    public void LinkNumberOfChannels(InputChannel input)
    {
        _linkedInput = input;
    }

    public void UnlinkNumberOfChannels()
    {
        _linkedInput = null;
    }

    public int GetNumberOfChannels()
    {
        return _linkedInput?.ConnectedFrom.Count > 0 ? _linkedInput.Samples.Count : _numberChannels;
    }

    public override string ToString()
    {
        return $"{Node}Output #{Index} - ";
    }

}
