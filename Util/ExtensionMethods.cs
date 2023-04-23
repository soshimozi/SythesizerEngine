namespace SynthesizerEngine.Util;

public static class ExtensionMethods
{
    public static bool SafeCompare(this double a, double b)
    {
        return Math.Abs(a - b) < double.Epsilon;
    }
    
}