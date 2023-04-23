using NAudio.Wave;

namespace SynthesizerEngine.Core.Audio;

public class Provider : WaveProvider32
{
    public Sink Output { get; private set; }
    public readonly Scheduler Scheduler;
    private readonly Device _device;

    public Provider()
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

    public int GetWriteTime()
    { return _device.GetWriteTime(); }


}