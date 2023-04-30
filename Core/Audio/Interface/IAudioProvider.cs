namespace SynthesizerEngine.Core.Audio.Interface;

public interface IAudioProvider
{
    bool NeedTraverse { set; }
    int Channels { get; }
    int SampleRate { get; }

    int TotalWriteTime { get; }

}