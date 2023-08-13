using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class Delay : AudioNode
{
    private readonly double _maximumDelayTime;
    private readonly Automation _delayTime;

    private readonly List<double[]> _buffers;
    private int _readWriteIndex;

    public Delay(IAudioProvider provider, double maximumDelayTime, double delayTime = 1) : base(provider, 2, 1)
    {
        LinkNumberOfOutputChannels(0, 0);

        _maximumDelayTime = maximumDelayTime;
        _delayTime = new Automation(this, 1, delayTime);

        _buffers = new List<double[]>();
        _readWriteIndex = 0;
    }

    protected override void GenerateMix()
    {
        var input = Inputs[0];
        var output = Outputs[0];


        var delayTime = _delayTime.GetValue() * AudioProvider.SampleRate;

        var numberOfChannels = input.Samples.Count;

        for (var i = 0; i < numberOfChannels; i++)
        {
            if (i >= _buffers.Count)
            {
                var bufferSize = (int)_maximumDelayTime * AudioProvider.SampleRate;
                _buffers.Add(new double[bufferSize]);
            }

            var buffer = _buffers[i];
            output.Samples[i] = buffer[_readWriteIndex];
            buffer[_readWriteIndex] = input.Samples[i];
        }

        _readWriteIndex += 1;
        if (_readWriteIndex >= delayTime)
        {
            _readWriteIndex = 0;
        }
    }
}