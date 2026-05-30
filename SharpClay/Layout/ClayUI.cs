using SharpClay.Interop;

namespace SharpClay.Layout;

public static class ClayUI
{
    public static ClayElementScope Container(in ElementDeclaration declaration) => 
        new ClayElementScope(in declaration);

    public static ClayElementScope Panel(in ElementDeclaration declaration) => 
        new ClayElementScope(in declaration);
}
