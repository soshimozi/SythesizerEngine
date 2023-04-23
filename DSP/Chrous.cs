using SynthesizerEngine.Core.Audio;
using System;

namespace SynthesizerEngine.DSP;

public class Chorus : Node
{
    /*
     *
     * * @param type:Float units:ms min:0.0 delayTime Delay time of the chorus.
 * @param type:UInt depth Depth of the Chorus.
 * @param type:Float units:Hz min:0.0 freq The frequency of the LFO running the Chorus.
     *
     */

    private readonly Oscillator _oscillator;
    private readonly List<double> _buffer;

    private readonly double _delayTime;
    private readonly uint _depth;

    private int _bufferPos;

    public Chorus(Provider provider, double delayTime = 30, uint depth = 3, float frequency = 0.1f) : base(provider, 1, 1)
    {
        _oscillator = new Oscillator(provider, frequency);

        _delayTime = delayTime;
        _depth = depth;

        _buffer = new List<double>();
        _bufferPos = 0;
        var l = AudioProvider.WaveFormat.SampleRate * 0.1;
        for (var i = 0; i < l; i++)
        {
            _buffer.Add(0.0);
        }
    }

    protected override void GenerateMix()
    {
        var input = Inputs[0];
        var output = Outputs[0];
        
        var numberOfChannels = input.Samples.Count;
        for (var i = 0; i < numberOfChannels; i++)
        {
            var s = input.Samples[i];

            if (++_bufferPos >= _buffer.Count)
            {
                _bufferPos = 0;
            }

            _buffer[_bufferPos] = s;
            _oscillator.Tick();

            var delay = _delayTime + _oscillator.Outputs[0].Samples[0] * _depth;
            delay *= AudioProvider.WaveFormat.SampleRate / 1000.0;
            delay = _bufferPos - Math.Floor(delay);
            while (delay < 0)
            {
                delay += _buffer.Count;
            }

            var sample = _buffer[(int)delay];
            output.Samples[i] = sample;
        }
    }
}