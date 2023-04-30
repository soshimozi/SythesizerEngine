namespace SynthesizerEngine.Core.Audio.Interface;

public interface IGroupNode
{
    List<IAudioNode> InputPassThroughNodes { get; }
    List<IAudioNode> OutputPassThroughNodes { get; set; }
}