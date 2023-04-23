using SynthesizerEngine.Core.Audio;

namespace SynthesizerEngine.Core;

/// <summary>
///
/// A specialized type of AudioNode where values from the inputs are passed
/// straight to the corresponding outputs in the most efficient way possible.
/// PassThroughNodes are used in AudioGroups to provide the inputs and
/// outputs, and can also be used in analysis nodes where no modifications to
/// the incoming audio are made.
/// </summary>
public class PassThroughNode : Node
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider">the audio provider object</param>
    /// <param name="numberOfInputs">the number of inputs</param>
    /// <param name="numberOfOutputs">the number of outputs</param>
    public PassThroughNode(Provider provider, int numberOfInputs, int numberOfOutputs)
        : base(provider, numberOfInputs, numberOfOutputs)
    {
    }

    /// <summary>
    ///
    /// Create output samples for each channel, copying any input samples to
    /// the corresponding outputs.
    ///
    /// 
    /// </summary>
    protected override void CreateOutputSamples()
    {
        var numberOfOutputs = Outputs.Count;
        for (var i = 0; i < numberOfOutputs; i++)
        {
            var input = Inputs[i];
            var output = Outputs[i];

            if (input.Samples.Count != 0)
            {
                output.Samples = input.Samples;
            }
            else
            {
                var numberOfChannels = output.GetNumberOfChannels();
                if (output.Samples.Count == numberOfChannels)
                {
                    continue;
                }
                else if (output.Samples.Count > numberOfChannels)
                {
                    output.Samples.RemoveRange(numberOfChannels, output.Samples.Count - numberOfChannels);
                    continue;
                }

                for (var j = output.Samples.Count; j < numberOfChannels; j++)
                {
                    output.Samples.Add(0);
                }
            }
        }
    }
}
