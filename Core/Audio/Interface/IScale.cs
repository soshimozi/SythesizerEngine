namespace SynthesizerEngine.Core.Audio.Interface;

public interface IScale
{
    double GetFrequency(int degree, double rootFrequency, int octave);
}