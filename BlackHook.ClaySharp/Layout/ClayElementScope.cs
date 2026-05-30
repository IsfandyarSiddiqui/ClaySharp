using ClaySharp.Interop;

namespace ClaySharp.Layout;

public readonly unsafe ref struct ClayElementScope
{
    public ClayElementScope(in ElementDeclaration declaration)
    {
        ClayNative.OpenElement();
        ClayNative.ConfigureOpenElementPtr(in declaration);
    }

    public void Dispose()
    {
        ClayNative.CloseElement();
    }
}
