using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.Core.Audio;

public class OutputChannel : IChannel
{
    public IAudioNode Node { get; }
    public int Index { get; }
    public List<IChannel> Connected { get; }
    public List<double> Samples { get; set; }

    private IChannel? _linkedInput;

    public int Channels
    {
        set => _numberChannels = value;
        get => GetNumberOfChannels();
    }

    private int _numberChannels;

    public OutputChannel(IAudioNode node, int index)
    {
        Node = node;
        Index = index;
        Connected = new List<IChannel>();
        Samples = new List<double>();

        _linkedInput = null;
        _numberChannels = 1;
    }

    public void Connect(IChannel input)
    {
        Connected.Add(input);
    }

    public void Disconnect(IChannel input)
    {
        Connected.Remove(input);
    }

    public void LinkNumberOfChannels(IChannel input)
    {
        _linkedInput = input;
    }


    private int GetNumberOfChannels()
    {
        var samples = _linkedInput?.Samples.Count ?? 0;
        return _linkedInput?.Connected.Count > 0 ? samples : _numberChannels;
    }
}
