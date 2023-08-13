using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class FeedbackDelay : AudioNode
{
    private readonly double _maximumDelayTime;
    private readonly Automation _delayTime;
    private readonly Automation _feedback;
    private readonly Automation _mix;

    private readonly List<double[]> _buffers;
    private int _readWriteIndex;

    public FeedbackDelay(IAudioProvider provider, float maximumDelayTime, float delayTime = 1, float feedback = 0.5f, float mix = 1)
        : base(provider, 4, 1)
    {
        LinkNumberOfOutputChannels(0, 0);
        _maximumDelayTime = maximumDelayTime;
        _delayTime = new Automation(this, 1, delayTime);
        _feedback = new Automation(this, 2, feedback);
        _mix = new Automation(this, 3, mix);

        _buffers = new List<double[]>();
        _readWriteIndex = 0;
    }

    protected override void GenerateMix()
    {
        var input = Inputs[0];
        var output = Outputs[0];

        
        var delayTime = _delayTime.GetValue() * AudioProvider.SampleRate;
        var feedback = _feedback.GetValue();
        var mix = _mix.GetValue();

        var numberOfChannels = input.Samples.Count;
        var numberOfBuffers = _buffers.Count;
        for (var i = 0; i < numberOfChannels; i++)
        {
            if (i >= numberOfBuffers)
            {
                var bufferSize = (int)(_maximumDelayTime * AudioProvider.SampleRate);
                _buffers.Add(new double[bufferSize]);
            }

            var buffer = _buffers[i];

            var inputSample = input.Samples[i];
            var bufferSample = buffer[_readWriteIndex];

            output.Samples[i] = mix * bufferSample + (1 - mix) * inputSample;
            buffer[_readWriteIndex] = inputSample + feedback * bufferSample;
        }

        _readWriteIndex += 1;
        if (_readWriteIndex >= delayTime)
        {
            _readWriteIndex = 0;
        }
    }

    public override string ToString()
    {
        return "Feedback Delay";
    }
}
