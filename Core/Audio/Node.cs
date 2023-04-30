using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.Core.Audio;

public abstract class Node : IAudioNode
{
    public IList<IChannel> Inputs { get; }
    public IList<IChannel> Outputs { get; }

    protected readonly IAudioProvider AudioProvider;
    private readonly Action _generate;

    protected Node(IAudioProvider provider, int numberOfInputs, int numberOfOutputs, Action? generate = null)
    {
        AudioProvider = provider;

        Inputs = new List<IChannel>();
        for (var i = 0; i < numberOfInputs; i++)
        {
            Inputs.Add(new InputChannel(this, i));
        }

        Outputs = new List<IChannel>();
        for (var i = 0; i < numberOfOutputs; i++)
        {
            Outputs.Add(new OutputChannel(this, i));
        }

        _generate = GenerateMix;

        if (generate != null)
        {
            _generate = generate;
        }
    }

    public virtual void Connect(IAudioNode node, int outputIndex = 0, int inputIndex = 0)
    {
        if (node is GroupNode nodeGroup)
        {
            var outputPin = Outputs[outputIndex];
            node = nodeGroup.InputPassThroughNodes[inputIndex];

            var inputPin = node.Inputs[0];
            outputPin.Connect(inputPin);
            inputPin.Connect(outputPin);
        }
        else
        {
            var outputPin = Outputs[outputIndex];
            var inputPin = node.Inputs[inputIndex];
            outputPin.Connect(inputPin);
            inputPin.Connect(outputPin);
        }


        AudioProvider.NeedTraverse = true;
    }

    public virtual void Disconnect(IAudioNode node, int output = 0, int input = 0)
    {
        if (node is GroupNode nodeGroup)
        {
            var outputPin = Outputs[output];
            var newNode = nodeGroup.InputPassThroughNodes[input];

            var inputPin = newNode.Inputs[0];
            inputPin.Disconnect(outputPin);
            outputPin.Disconnect(inputPin);
        }
        else
        {
            var outputPin = Outputs[output];
            var inputPin = node.Inputs[input];
            inputPin.Disconnect(outputPin);
            outputPin.Disconnect(inputPin);
        }

        AudioProvider.NeedTraverse = true;
    }

    public virtual void Tick()
    {
        CreateInputSamples();
        CreateOutputSamples();

        _generate();
    }

    public virtual void Remove()
    {
        // Disconnect inputs
        foreach (var input in Inputs)
        {
            foreach (var outputPin in input.Connected)
            {
                var output = outputPin.Node;
                output.Disconnect(this, outputPin.Index, input.Index);
            }
        }

        // Disconnect outputs
        foreach (var output in Outputs)
        {
            foreach (var inputPin in output.Connected)
            {
                var input = inputPin.Node;
                Disconnect(input, output.Index, inputPin.Index);
            }
        }
    }

    public abstract void GenerateMix();

    public virtual void CreateInputSamples()
    {
        foreach (var input in Inputs)
        {
            var numberOfInputChannels = 0;

            input.Samples.Clear();
            foreach (var output in input.Connected)
            {
                for (var k = 0; k < output.Samples.Count; k++)
                {
                    var sample = output.Samples[k];
                    if (k < numberOfInputChannels)
                    {
                        input.Samples[k] += sample;
                    }
                    else
                    {
                        input.Samples.Add(sample);
                        numberOfInputChannels += 1;
                    }
                }
            }

            if (input.Samples.Count > numberOfInputChannels)
            {
                input.Samples.RemoveRange(numberOfInputChannels, input.Samples.Count - numberOfInputChannels);
            }
        }
    }

    public virtual void CreateOutputSamples()
    {

        foreach (var output in Outputs)
        {
            output.Samples.Clear();

            var numberOfChannels = output.Channels;
            if (output.Samples.Count == numberOfChannels)
            {
                continue;
            }

            if (output.Samples.Count > numberOfChannels)
            {
                output.Samples.RemoveRange(numberOfChannels, output.Samples.Count - numberOfChannels);
                continue;
            }

            for (var j = output.Samples.Count; j < numberOfChannels; j++)
            {
                output.Samples.Add(0);
            }
        }
    }

    public List<IAudioNode> Traverse(List<IAudioNode> nodes)
    {
        if (nodes.Contains(this)) return nodes;
        nodes.Add(this);
        nodes = TraverseParents(nodes);
        return nodes;
    }

    private List<IAudioNode> TraverseParents(List<IAudioNode> nodes)
    {
        return Inputs
            .SelectMany(input => input.Connected)
            .Aggregate(nodes, (current, stream) => stream.Node.Traverse(current));
    }

    protected void SetNumberOfOutputChannels(int output, int numberOfChannels)
    {
        Outputs[output].Channels = numberOfChannels;
    }

    protected void LinkNumberOfOutputChannels(int output, int input)
    {
        Outputs[output].LinkNumberOfChannels(Inputs[input]);
    }

}