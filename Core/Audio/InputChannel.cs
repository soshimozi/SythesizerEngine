namespace SynthesizerEngine.Core.Audio;


public class InputChannel
{
    public Node Node { get; }
    public int Index { get; }
    public List<OutputChannel> ConnectedFrom { get; }
    public List<double> Samples { get; private set; }

    public InputChannel(Node node, int index)
    {
        Node = node;
        Index = index;
        ConnectedFrom = new List<OutputChannel>();
        Samples = new List<double>();
    }

    public void Connect(OutputChannel output)
    {
        ConnectedFrom.Add(output);
    }

    public void Disconnect(OutputChannel output)
    {
        ConnectedFrom.Remove(output);
        if (ConnectedFrom.Count == 0)
        {
            Samples = new List<double>();
        }
    }

    public override string ToString()
    {
        return $"{Node}Input #{Index}";
    }
}
