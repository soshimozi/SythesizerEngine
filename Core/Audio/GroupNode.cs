using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.Core.Audio;

public class GroupNode : Node, IGroupNode
{
    public List<IAudioNode> InputPassThroughNodes { get; }
    public List<IAudioNode> OutputPassThroughNodes { get; set; }

    protected GroupNode(IAudioProvider lib, int numberOfInputs, int numberOfOutputs) : base(lib, numberOfInputs, numberOfOutputs)
    {
        InputPassThroughNodes = new List<IAudioNode>();
        for (var i = 0; i < numberOfInputs; i++)
        {
            InputPassThroughNodes.Add(new PassThroughNode(lib, 1, 1));
        }

        OutputPassThroughNodes = new List<IAudioNode>();
        for (var i = 0; i < numberOfOutputs; i++)
        {
            OutputPassThroughNodes.Add(new PassThroughNode(lib, 1, 1));
        }
    }

    public override void Connect(IAudioNode node, int output = 0, int input = 0)
    {
        var psNode = OutputPassThroughNodes[output] as PassThroughNode;
        psNode?.Connect(node, 0, input);
    }

    public override void Disconnect(IAudioNode node, int output = 0, int input = 0)
    {
        OutputPassThroughNodes[output].Disconnect(node, 0, input);
    }

    public override void Remove()
    {
        var numberOfInputs = InputPassThroughNodes.Count;

        for (var i = 0; i < numberOfInputs; i++)
        {
            (InputPassThroughNodes[i] as PassThroughNode)?.Remove();
        }

        var numberOfOutputs = OutputPassThroughNodes.Count;
        for (var i = 0; i < numberOfOutputs; i++)
        {
            (OutputPassThroughNodes[i] as PassThroughNode)?.Remove();
        }
    }

    public override void GenerateMix()
    {
        // do nothing
    }
}
