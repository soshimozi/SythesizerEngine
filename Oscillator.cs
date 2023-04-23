using System;
using NAudio.Wave;
using NAudio.Dsp;

namespace SynthEngine;

using System;
using System.Collections.Generic;

public class Oscillator : Generator
{
    private const double FullPI = Math.PI * 2;

    public double PhaseOffset { get; set; }
    public double PulseWidth { get; set; }
    public double Fm { get; set; }
    public WaveShape Shape { get; set; }

    public double Phase { get; set; }
    private double _p { get; set; }
    public double Frequency { get; set; }
    public float[] WaveTable { get; set; }
    public int SampleRate { get; set; }

    private Dictionary<WaveShape, Func<double>> ShapeFunctions { get; set; }

    private Generator? _fmGenerator;
    public void ConnectFM(Generator fmGenerator)
    {
        _fmGenerator = fmGenerator;
    }

 
    public Oscillator(int sampleRate = 44100, double freq = 440, WaveShape waveShape = WaveShape.Sine)
        : base()
    {
        PhaseOffset = 0;
        PulseWidth = 0.5;
        Fm = 0;
        Shape = waveShape;
        Mix = 1;

        Phase = 0;
        _p = 0;

        Frequency = double.IsNaN(freq) ? 440 : freq;
        WaveTable = new float[1];
        SampleRate = sampleRate;

        ShapeFunctions = new Dictionary<WaveShape, Func<double>>
        {
            { WaveShape.Sine, Sine },
            { WaveShape.Triangle, Triangle },
            { WaveShape.Sawtooth, Sawtooth },
            { WaveShape.Square, Square },
            { WaveShape.InvSawtooth, InvSawtooth },
            { WaveShape.Pulse, Pulse }
        };
    }

    public override void Generate()
    {
        _fmGenerator?.Generate();

        var f = Frequency;
        var fmMix = _fmGenerator?.Mix ?? 0;
        var fmValue = _fmGenerator?.GetChannel(1) ?? 0;

        var fmMod = fmValue * fmMix;

        f += f * fmMod;
        Phase = (Phase + f / SampleRate / 2) % 1;
        var p = (Phase + PhaseOffset) % 1;
        _p = p < PulseWidth ? p / PulseWidth : (p - PulseWidth) / (1 - PulseWidth);
    }

    public override float GetChannel(int channel)
    {
        return (float)ShapeFunctions[Shape]();
    }

    public double GetPhase => _p;

    public void Reset(double? p)
    {
        if (p == null) Phase = _p = 0;

        Phase = _p = double.IsNaN(p.GetValueOrDefault()) ? 0 : p.GetValueOrDefault();
    }

    public override void Reset()
    {
        Reset(null);
    }

    public bool SetWavetable(float[] wt)
    {
        WaveTable = wt;
        return true;
    }

    public double Sine() => Math.Sin(_p * FullPI);

    public double Triangle() => _p < 0.5 ? 4 * _p - 1 : 3 - 4 * _p;

    public double Square() => _p < 0.5 ? -1 : 1;

    public double Sawtooth() => 1 - _p * 2;

    public double InvSawtooth() => _p * 2 - 1;

    public double Pulse() => _p < 0.5 ? (_p < 0.25 ? _p * 8 - 1 : 1 - (_p - 0.25) * 8) : -1;

    public Func<double> AddWaveShape(WaveShape shape, Func<double> algorithm)
    {
        if (algorithm != null)
        {
            ShapeFunctions[shape] = algorithm;
        }

        return ShapeFunctions[shape];
    }

    public Func<double> CreateMixWave(WaveShape shape, List<(WaveShape shape, double mix)> waveshapes)
    {
        return AddWaveShape(shape, () =>
        {
            double sample = 0;
            foreach (var waveshape in waveshapes)
            {
                sample += ShapeFunctions[waveshape.shape]() * waveshape.mix;
            }

            return sample;
        });
    }
}
