using ClaySharp.Interop;

namespace ClaySharp.Layout;

public static class ClaySizing
{
    public static SizingAxis Axis(SizingType type, float value = 0, float min = 0, float max = 0)
    {
        return new SizingAxis
        {
            Type = type,
            Size = new SizingAxisValue
            {
                Percent = value,
                MinMax = new SizingMinMax { Min = min, Max = max }
            }
        };
    }

    public static Sizing Axes(
        SizingType widthType,
        float widthValue,
        float widthMin,
        float widthMax,
        SizingType heightType,
        float heightValue,
        float heightMin,
        float heightMax)
    {
        return new Sizing
        {
            Width = Axis(widthType, widthValue, widthMin, widthMax),
            Height = Axis(heightType, heightValue, heightMin, heightMax)
        };
    }
}
