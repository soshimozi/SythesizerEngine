namespace SynthesizerEngine.Core.Audio.Interface;

public interface IAudioNode
{
    IList<IChannel> Inputs { get; }
    void Disconnect(IAudioNode node, int output, int input);
    void Tick();
    List<IAudioNode> Traverse(List<IAudioNode> nodes);
}