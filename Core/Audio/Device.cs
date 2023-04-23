namespace SynthesizerEngine.Core.Audio;


public class Device : Node
{
    private List<Node> _nodes;

    public bool NeedTraverse { get; set; }
    private readonly Provider _provider;
    private int _writePosition = 0;

    public Device(Provider lib) : base(lib, 1, 0)
    {
        _provider = lib;
        _nodes = new List<Node>();
    }

    public override string ToString()
    {
        return "Audio Output Device";
    }

    public void Tick(float[] buffer, int offset, int sampleCount)
    {
        var input = Inputs[0];

        for (var i = 0; i < sampleCount; i += _provider.WaveFormat.Channels)
        {
            if (NeedTraverse)
            {
                _nodes = Traverse(new List<Node>());
                NeedTraverse = false;
            }

            for (var j = _nodes.Count - 1; j > 0; j--)
            {
                _nodes[j].Tick();
            }

            CreateInputSamples();


            for (var j = 0; j < _provider.WaveFormat.Channels; j++)
            {
                buffer[offset + i + j] = (float)input.Samples[j];
            }

            _writePosition++;
        }
    }

    public int GetWriteTime()
    {
        return _writePosition;
    }

}
