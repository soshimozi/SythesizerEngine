using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.Core.Audio;


public class Device : AudioNode
{
    private List<IAudioNode> _nodes;

    public bool NeedTraverse { get; set; }
    private int _writePosition = 0;

    public Device(IAudioProvider audioProvider) : base(audioProvider, 1, 0)
    {
        _nodes = new List<IAudioNode>();
    }

    public void Read(float[] buffer, int offset, int sampleCount)
    {
        var input = Inputs[0];

        for (var i = 0; i < sampleCount; i += AudioProvider.Channels)
        {
            if (NeedTraverse)
            {
                _nodes = Traverse(new List<IAudioNode>());
                NeedTraverse = false;
            }

            for (var j = _nodes.Count - 1; j > 0; j--)
            {
                _nodes[j].Tick();
            }

            MigrateInputSamples();


            for (var j = 0; j < AudioProvider.Channels; j++)
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
