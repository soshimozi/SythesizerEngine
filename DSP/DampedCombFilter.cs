using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;
using SynthesizerEngine.Core.Audio.Interface;

namespace SynthesizerEngine.DSP;

public class DampedCombFilter: AudioNode
{
    private readonly Automation _delayTime;
    private readonly Automation _decayTime;
    private readonly Automation _damping;

    private readonly List<double[]> _buffers;
    private readonly List<double> _filterStores;

    private readonly double _maximumDelayTime;
    private int _readWriteIndex;

    protected DampedCombFilter(IAudioProvider provider, double maximumDelayTime, double delayTime = 1, double decayTime = 0, double damping = 0) : base(provider, 4, 1)
    {
        LinkNumberOfOutputChannels(0, 0);

        _maximumDelayTime = maximumDelayTime;

        _delayTime = new Automation(this, 1, delayTime);
        _decayTime = new Automation(this, 2, decayTime);
        _damping = new Automation(this, 3, damping);

        _buffers = new List<double[]>();
        _filterStores = new List<double>();

    }

    protected override void GenerateMix()
    {
        var input = Inputs[0];
        var output = Outputs[0];

        var delayTime = _delayTime.GetValue() * AudioProvider.SampleRate;
        var decayTime = _decayTime.GetValue() * AudioProvider.SampleRate;
        var damping = _damping.GetValue();
        var feedback = Math.Exp(-3 * delayTime / decayTime);

        var numberOfChannels = input.Samples.Count;
        for (var i = 0; i < numberOfChannels; i++)
        {
            if (i >= _buffers.Count)
            {
                var bufferSize = (int) (_maximumDelayTime * AudioProvider.SampleRate);
                _buffers.Add(new double[bufferSize]);
            }

            if (i >= _filterStores.Count)
            {
                _filterStores.Add(0);
            }

            var buffer = _buffers[i];
            var filterStore = _filterStores[i];

            var outputValue = buffer[_readWriteIndex];
            filterStore = (outputValue * (1 - damping)) + (filterStore * damping);

            output.Samples[i] = outputValue;
            buffer[_readWriteIndex] = input.Samples[i] + feedback * filterStore;

            _filterStores[i] = filterStore;
        }

        _readWriteIndex += 1;
        if (_readWriteIndex >= delayTime)
        {
            _readWriteIndex = 0;
        }
    }
}