using System.Runtime.CompilerServices;
using SynthesizerEngine.Core.Audio.Interface;
using System.Xml.Linq;

namespace SynthesizerEngine.Core.Audio;

public class AudioNode : IAudioNode
{
    public IList<IChannel> Inputs { get; }
    public IList<IChannel> Outputs { get; }

    public List<IAudioNode> InputPassThroughNodes { get; }
    public List<IAudioNode> OutputPassThroughNodes { get; set; }

    public bool UsesPassThrough { get; }

    protected readonly IAudioProvider AudioProvider;

    private readonly bool _isVirtual;

    protected AudioNode(IAudioProvider provider, int numberOfInputs, int numberOfOutputs, bool usesPassThrough = false, bool isVirtual = false)
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

        InputPassThroughNodes = new List<IAudioNode>();
        OutputPassThroughNodes = new List<IAudioNode>();

        _isVirtual = isVirtual;
        UsesPassThrough = usesPassThrough;


        if (!UsesPassThrough) return;

        // need pass through nodes - be careful here.  PassThroughNode is an AudioNode, so this constructor will be called again
        // if you always create pass through nodes it will end up in an stack overflow
        for (var i = 0; i < numberOfInputs; i++)
        {
            InputPassThroughNodes.Add(new AudioNode(AudioProvider, 1, 1, isVirtual: true));
        }

        for (var i = 0; i < numberOfOutputs; i++)
        {
            OutputPassThroughNodes.Add(new AudioNode(AudioProvider, 1, 1, isVirtual: true));
        }
    }


    public virtual void Connect(IAudioNode node, int outputIndex = 0, int inputIndex = 0)
    {
        if (UsesPassThrough)
        {
            var psNode = OutputPassThroughNodes[outputIndex];
            psNode.Connect(node, 0, inputIndex);
        }
        else
        {
            var inputPin = node.UsesPassThrough ? node.InputPassThroughNodes[inputIndex].Inputs[0] : node.Inputs[inputIndex];
            var outputPin = Outputs[outputIndex];

            outputPin.Connect(inputPin);
            inputPin.Connect(outputPin);
        }

        AudioProvider.NeedTraverse = true;
    }

    public virtual void Disconnect(IAudioNode fromNode, int output = 0, int input = 0)
    {
        if (UsesPassThrough)
        {
            OutputPassThroughNodes[output].Disconnect(fromNode, 0, input);
        }
        else
        {
            var inputPin = fromNode.UsesPassThrough ? fromNode.InputPassThroughNodes[input].Inputs[0] : fromNode.Inputs[input];
            var outputPin = Outputs[output];


            inputPin.Disconnect(outputPin);
            outputPin.Disconnect(inputPin);

            AudioProvider.NeedTraverse = true;
        }
    }

    public virtual void Tick()
    {
        MigrateInputSamples();
        MigrateOutputSamples();

        GenerateMix();
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

        foreach(var passThrough in InputPassThroughNodes)
        {
            passThrough.Remove();
        }

        foreach (var passThrough in OutputPassThroughNodes)
        {
            passThrough.Remove();
        }

    }

    protected virtual void GenerateMix()
    {
        // do nothing by default
    }

    protected void MigrateInputSamples()
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

    private void MigrateOutputSamples()
    {

        if (_isVirtual)
        {
            CreateVirtualOutputSamples();
            return;
        }

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

    private void CreateVirtualOutputSamples()
    {
        var numberOfOutputs = Outputs.Count;
        for (var i = 0; i < numberOfOutputs; i++)
        {
            var input = Inputs[i];
            var output = Outputs[i];

            if (input.Samples.Count != 0)
            {
                output.Samples = input.Samples;
            }
            else
            {
                var numberOfChannels = output.Channels;
                if (output.Samples.Count == numberOfChannels)
                {
                    continue;
                }
                else if (output.Samples.Count > numberOfChannels)
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

    /// <summary>
    ///  Link an output to an input, forcing the output to always contain the
    ///  same number of channels as the input.
    /// </summary>
    /// <param name="output"></param>
    /// <param name="input"></param>
    protected void LinkNumberOfOutputChannels(int output, int input)
    {
        Outputs[output].LinkNumberOfChannels(Inputs[input]);
    }

}