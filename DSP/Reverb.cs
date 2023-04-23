using SynthesizerEngine.Core;
using SynthesizerEngine.Core.Audio;

namespace SynthesizerEngine.DSP;

public class Reverb : Node
{
    private const float InitialMix = 0.33f;
    private const float FixedGain = 0.015f;
    private const float InitialDamping = 0.5f;
    private const float ScaleDamping = 0.4f;
    private const float InitialRoomSize = 0.5f;
    private const float ScaleRoom = 0.28f;
    private const float OffsetRoom = 0.7f;

    private readonly int[] _combTuning;
    private readonly int[] _allPassTuning;

    private readonly Automation _mix;
    private readonly Automation _roomSize;
    private readonly Automation _damping;

    private readonly List<double[]> _combBuffers;
    private readonly List<int> _combIndices;
    private readonly List<double> _filterStores;

    private readonly List<double[]> _allPassBuffers;
    private readonly List<int> _allPassIndices;

    public Reverb(Provider provider, double? mix = null, double? roomSize = null, double? damping = null)
        : base(provider, 4, 1)
    {
        // Parameters: for 44.1k or 48k
        _combTuning = new[] { 1116, 1188, 1277, 1356, 1422, 1491, 1557, 1617 };
        _allPassTuning = new[] { 556, 441, 341, 225 };

        // Controls
        _mix = new Automation(this, 1, mix ?? InitialMix);
        _roomSize = new Automation(this, 2, roomSize ?? InitialRoomSize);
        _damping = new Automation(this, 3, damping ?? InitialDamping);

        // Damped comb filters
        _combBuffers = new List<double[]>();
        _combIndices = new List<int>();
        _filterStores = new List<double>();

        var numberOfCombs = _combTuning.Length;
        for (var i = 0; i < numberOfCombs; i++)
        {
            _combBuffers.Add(new double[_combTuning[i]]);
            _combIndices.Add(0);
            _filterStores.Add(0);
        }

        // All-pass filters
        _allPassBuffers = new List<double[]>();
        _allPassIndices = new List<int>();

        var numberOfFilters = _allPassTuning.Length;
        for (var i = 0; i < numberOfFilters; i++)
        {
            _allPassBuffers.Add(new double[_allPassTuning[i]]);
            _allPassIndices.Add(0);
        }
    }

    protected override void GenerateMix()
    {
        var mix = _mix.GetValue();
        var roomSize = _roomSize.GetValue();
        var damping = _damping.GetValue();

        var numberOfCombs = _combTuning.Length;
        var numberOfFilters = _allPassTuning.Length;

        var value = Inputs[0].Samples[0];
        var dryValue = value;

        value *= FixedGain;
        var gainedValue = value;

        damping *= ScaleDamping;
        var feedback = roomSize * ScaleRoom + OffsetRoom;

        for (var i = 0; i < numberOfCombs; i++)
        {
            var combIndex = _combIndices[i];
            var combBuffer = _combBuffers[i];
            var filterStore = _filterStores[i];

            var output = combBuffer[combIndex];
            filterStore = (output * (1 - damping)) + (filterStore * damping);
            value += output;
            combBuffer[combIndex] = gainedValue + feedback * filterStore;

            combIndex++;
            if (combIndex >= combBuffer.Length)
            {
                combIndex = 0;
            }

            _combIndices[i] = combIndex;
            _filterStores[i] = filterStore;
        }

        for (var i = 0; i < numberOfFilters; i++)
        {
            var allPassBuffer = _allPassBuffers[i];
            var allPassIndex = _allPassIndices[i];

            var input = value;
            var bufferValue = allPassBuffer[allPassIndex];
            value = -value + bufferValue;
            allPassBuffer[allPassIndex] = input + (bufferValue * 0.5f);

            allPassIndex++;
            if (allPassIndex >= allPassBuffer.Length)
            {
                allPassIndex = 0;
            }

            _allPassIndices[i] = allPassIndex;
        }

        Outputs[0].Samples[0] = mix * value + (1 - mix) * dryValue;
    }

}