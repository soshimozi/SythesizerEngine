using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;

namespace SynthesizerEngine.DSP;

/// <summary>
/// 
///
/// A simple (and frankly shoddy) zero-lookahead limiter.
/// 
///  **Inputs**
/// 
///  - Audio
///  - Threshold
///  - Attack
///  - Release
/// 
///  **Outputs**
/// 
///  - Limited audio
/// 
///  **Parameters**
/// 
///  - threshold The limiter threshold.  Linked to input 1.
///  - attack The attack time in seconds. Linked to input 2.
///  - release The release time in seconds.  Linked to input 3.
/// 
///
/// </summary>
public class Limiter : Node
{
    private readonly List<double> _followers = new List<double>();
    private readonly Automation _threshold;
    private readonly Automation _attack;
    private readonly Automation _release;

    public Limiter(Provider provider, double threshold = 0.95, double attack = 0.01, double release = 0.4) :
        base(provider, 4, 1)
    {
        LinkNumberOfOutputChannels(0, 0);

        _threshold = new Automation(this, 1, threshold);
        _attack = new Automation(this, 2, attack);
        _release = new Automation(this, 2, release);
    }

    protected override void GenerateMix()
    {
        var input = Inputs[0];
        var output = Outputs[0];

        // Local processing variables
        var attack = Math.Pow(0.01, 1 / (_attack.GetValue() *
                                         AudioProvider.WaveFormat.SampleRate));
        var release = Math.Pow(0.01, 1 / (_release.GetValue() *
                                          AudioProvider.WaveFormat.SampleRate));

        var threshold = _threshold.GetValue();

        var numberOfChannels = input.Samples.Count;
        for (var i = 0; i < numberOfChannels; i++)
        {
            if (i >= _followers.Count)
            {
                _followers.Add(0);
            }

            var follower = _followers[i];

            var value = input.Samples[i];

            // Calculate amplitude envelope
            var absValue = Math.Abs(value);
            if (absValue > follower)
            {
                follower = attack * (follower - absValue) + absValue;
            }
            else
            {
                follower = release * (follower - absValue) + absValue;
            }

            var diff = follower - threshold;
            if (diff > 0)
            {
                output.Samples[i] = value / (1 + diff);
            }
            else
            {
                output.Samples[i] = value;
            }

            _followers[i] = follower;
        }
    }
}