namespace SynthesizerEngine.Core.Audio.Interface;

public interface IAudioNode
{
    IList<IChannel> Inputs { get; }
    IList<IChannel> Outputs { get; }

    void Disconnect(IAudioNode node, int output, int input);
    void Connect(IAudioNode node, int outputIndex, int inputIndex);

    void Tick();
    List<IAudioNode> Traverse(List<IAudioNode> nodes);

    List<IAudioNode> InputPassThroughNodes { get; }
    List<IAudioNode> OutputPassThroughNodes { get; set; }

    void Remove();
    bool UsesPassThrough { get; }
}