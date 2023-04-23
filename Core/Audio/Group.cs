namespace SynthesizerEngine.Core.Audio;

public class Group : Node
{
    public List<PassThroughNode> InputPassThroughNodes { get; private set; }
    protected List<PassThroughNode> OutputPassThroughNodes { get; set; }

    protected Group(Provider lib, int numberOfInputs, int numberOfOutputs) : base(lib, numberOfInputs, numberOfOutputs)
    {
        InputPassThroughNodes = new List<PassThroughNode>();
        for (var i = 0; i < numberOfInputs; i++)
        {
            InputPassThroughNodes.Add(new PassThroughNode(lib, 1, 1));
        }

        OutputPassThroughNodes = new List<PassThroughNode>();
        for (var i = 0; i < numberOfOutputs; i++)
        {
            OutputPassThroughNodes.Add(new PassThroughNode(lib, 1, 1));
        }
    }

    public override void Connect(Node node, int output = 0, int input = 0)
    {
        var psNode = OutputPassThroughNodes[output];
        psNode.Connect(node, 0, input);
    }

    public override void Disconnect(Node node, int output = 0, int input = 0)
    {
        OutputPassThroughNodes[output].Disconnect(node, 0, input);
    }

    public override void Remove()
    {
        var numberOfInputs = InputPassThroughNodes.Count;
        for (var i = 0; i < numberOfInputs; i++)
        {
            InputPassThroughNodes[i].Remove();
        }

        var numberOfOutputs = OutputPassThroughNodes.Count;
        for (var i = 0; i < numberOfOutputs; i++)
        {
            OutputPassThroughNodes[i].Remove();
        }
    }

}
