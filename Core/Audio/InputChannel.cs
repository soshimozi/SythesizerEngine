using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.Core.Audio;


public class InputChannel : IChannel
{
    public IAudioNode Node { get; }
    public int Index { get; }
    public List<IChannel> Connected { get; }
    public int Channels { get; set; }
    public int TotalWriteTime { get; }
    public List<double> Samples { get; set; }

    public InputChannel(IAudioNode node, int index)
    {
        Node = node;
        Index = index;
        Connected = new List<IChannel>();
        Samples = new List<double>();
    }

    public void Connect(IChannel output)
    {
        Connected.Add(output);
    }

    public void Disconnect(IChannel output)
    {
        Connected.Remove(output);
        if (Connected.Count == 0)
        {
            Samples = new List<double>();
        }
    }

    public void LinkNumberOfChannels(IChannel input)
    {
        throw new NotImplementedException();
    }

    public bool NeedTraverse { get; set; }
    public int SampleRate { get; }

    public int GetNumberOfChannels()
    {
        throw new NotImplementedException();
    }
}
