namespace SynthesizerEngine.Pattern;

public class PatternBase
{
    public virtual object? Next()
    {
        return null;
    }

    protected object? ValueOf(object? item)
    {
        if (item is PatternBase p)
        {
            return p.Next();
        }
        else
        {
            return item;
        }
    }

    public virtual void Reset() { }

    public static double? ConvertValueToNumber(object? value)
    {
        var objectType = value?.GetType();

        if (objectType == typeof(double) || objectType == typeof(double?))
        {
            return (double?)value;
        }

        if (double.TryParse(value?.ToString(), out var result))
        {
            return result;
        }

        return null;
    }

}