using NAudio.Wave;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.Core.Audio;

public class AudioProvider : WaveProvider32, IAudioProvider
{
    public Sink Output { get; private set; }
    public readonly Scheduler Scheduler;
    private readonly Device _device;

    public AudioProvider()
    {
        _device = new Device(this);
        Scheduler = new Scheduler(this);
        Output = new Sink(this, _device, Scheduler);
    }

    public override int Read(float[] buffer, int offset, int count)
    {
        _device.Tick(buffer, offset, count);
        return count;
    }

    public bool NeedTraverse
    {
        set => _device.NeedTraverse = value;
    }

    public int Channels => WaveFormat.Channels;
    public int SampleRate => WaveFormat.SampleRate;

    public int TotalWriteTime => _device.GetWriteTime();

}