using System.Runtime.InteropServices;

namespace SharpClay.Interop;

/// <summary>
/// Opaque wrapper representing Clay's internal layout engine context context.
/// This state should only be accessed via pointers returned by <see cref="ClayNative.Clay_Initialize"/> or <see cref="ClayNative.GetCurrentContext"/>.
/// </summary>
public struct Context { }

/// <summary>
/// Contains pointer (e.g. mouse or touch) position and state information.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct PointerData
{
    /// <summary>
    /// The current 2D screen coordinates of the pointer.
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// The interaction/click state of the pointer on the current frame.
    /// </summary>
    public PointerDataInteractionState State;
}

/// <summary>
/// A native string wrapper used by Clay to represent text safely across the ABI.
/// </summary>
/// <remarks>
/// Because runtime marshalling is disabled, strings are passed as raw byte pointers (<see cref="Chars"/>) 
/// typically pointing to UTF-8 or ANSI encoded character arrays.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct ClayString
{
    /// <summary>
    /// Indicates if the string memory is statically allocated (e.g., constant string literal) and does not need to be freed.
    /// </summary>
    [MarshalAs(UnmanagedType.U1)]
    public bool IsStaticallyAllocated;

    /// <summary>
    /// The number of characters/bytes in the string.
    /// </summary>
    public int Length;

    /// <summary>
    /// A raw pointer to the null-terminated byte sequence in unmanaged memory.
    /// </summary>
    public byte* Chars;
}

/// <summary>
/// Represents a slice or segment of a string. Used by Clay's text layout engine during wrapping and text rendering.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct ClayStringSlice
{
    /// <summary>
    /// The length of the string slice in bytes.
    /// </summary>
    public int Length;

    /// <summary>
    /// A raw pointer to the start of the slice in unmanaged memory.
    /// </summary>
    public byte* Chars;

    /// <summary>
    /// A raw pointer to the base string from which this slice was carved.
    /// </summary>
    public byte* BaseChars;
}

/// <summary>
/// Defines a contiguous block of pre-allocated unmanaged memory that Clay uses for all structural and dynamic layout allocations.
/// </summary>
/// <remarks>
/// Developers must allocate a block of memory (e.g., via <see cref="Marshal.AllocHGlobal"/>) of at least <see cref="ClayNative.MinMemorySize"/>
/// and pass it to initialize the arena. Clay performs linear allocator increments internally within this block.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct Arena
{
    /// <summary>
    /// The current allocation offset pointer within the memory block.
    /// </summary>
    public UIntPtr NextAllocation;

    /// <summary>
    /// The total memory capacity of the arena in bytes.
    /// </summary>
    public UIntPtr Capacity;

    /// <summary>
    /// A raw pointer to the start of the backing unmanaged memory block.
    /// </summary>
    public byte* Memory;
}

/// <summary>
/// Represents standard 2D floating-point dimensions.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct Dimensions
{
    /// <summary>
    /// The width component in pixels.
    /// </summary>
    public float Width;

    /// <summary>
    /// The height component in pixels.
    /// </summary>
    public float Height;
}

/// <summary>
/// Represents a standard 2D vector.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct Vector2
{
    /// <summary>
    /// The horizontal X-axis component.
    /// </summary>
    public float X;

    /// <summary>
    /// The vertical Y-axis component.
    /// </summary>
    public float Y;
}

/// <summary>
/// Represents a color using four floating-point components (Red, Green, Blue, Alpha).
/// </summary>
/// <remarks>
/// Component values are typically represented in the range [0.0 - 255.0] matching native Clay conventions.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public partial struct Color
{
    /// <summary>Red channel intensity.</summary>
    public float R;
    /// <summary>Green channel intensity.</summary>
    public float G;
    /// <summary>Blue channel intensity.</summary>
    public float B;
    /// <summary>Alpha transparency channel.</summary>
    public float A;
}

/// <summary>
/// Defines a 2D bounding rectangle representing computed UI element placements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct BoundingBox
{
    /// <summary>The horizontal screen coordinate of the top-left corner.</summary>
    public float X;
    /// <summary>The vertical screen coordinate of the top-left corner.</summary>
    public float Y;
    /// <summary>The calculated layout width.</summary>
    public float Width;
    /// <summary>The calculated layout height.</summary>
    public float Height;
}

/// <summary>
/// Specifies how child elements are aligned within their parent container along both axes.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct ChildAlignment
{
    /// <summary>Horizontal alignment setting.</summary>
    public LayoutAlignmentX X;
    /// <summary>Vertical alignment setting.</summary>
    public LayoutAlignmentY Y;
}

/// <summary>
/// Defines the minimum and maximum constraints for element dimensions.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct SizingMinMax
{
    /// <summary>The minimum allowed size in pixels.</summary>
    public float Min;

    /// <summary>The maximum allowed size in pixels.</summary>
    public float Max;
}

/// <summary>
/// Emulates an anonymous C union representing a sizing constraint value.
/// Field offsets overlap so that it can store either percentage values or min/max bounds.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public partial struct SizingAxisValue
{
    /// <summary>
    /// Explicit pixel bounding limits. Valid if the sizing type is fixed or bound constrained.
    /// </summary>
    [FieldOffset(0)] public SizingMinMax MinMax;

    /// <summary>
    /// Percentage scale factor. Represented in range [0.0 - 1.0]. Valid if sizing type is set to percentage.
    /// </summary>
    [FieldOffset(0)] public float Percent;
}

/// <summary>
/// Specifies sizing rules and constraints for a single layout axis (either Width or Height).
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct SizingAxis
{
    /// <summary>The sizing constraint value (e.g. percentage or min/max pixels).</summary>
    public SizingAxisValue Size;

    /// <summary>The sizing mode type (Fit, Grow, Percent, Fixed).</summary>
    public SizingType Type;
}

/// <summary>
/// Defines layout sizing rules for both axes of a UI element.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct Sizing
{
    /// <summary>Sizing specifications for the horizontal X axis.</summary>
    public SizingAxis Width;

    /// <summary>Sizing specifications for the vertical Y axis.</summary>
    public SizingAxis Height;
}

/// <summary>
/// Defines inner spacing padding values surrounding the children of a container.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct Padding
{
    /// <summary>Left side padding thickness in pixels.</summary>
    public ushort Left;
    /// <summary>Right side padding thickness in pixels.</summary>
    public ushort Right;
    /// <summary>Top side padding thickness in pixels.</summary>
    public ushort Top;
    /// <summary>Bottom side padding thickness in pixels.</summary>
    public ushort Bottom;
}

/// <summary>
/// Defines general layout configurations for element arrangement, spacing, and sizing.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct LayoutConfig
{
    /// <summary>Axis sizing rules.</summary>
    public Sizing Sizing;

    /// <summary>Interior boundary padding values.</summary>
    public Padding Padding;

    /// <summary>Spaced gap distance in pixels between sequential child elements.</summary>
    public ushort ChildGap;

    /// <summary>Child alignment placement properties.</summary>
    public ChildAlignment ChildAlignment;

    /// <summary>Direction direction along which elements are appended (horizontal or vertical).</summary>
    public LayoutDirection LayoutDirection;
}

/// <summary>
/// Specifies corner rounding radii for rectangles, borders, and images.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct CornerRadius
{
    /// <summary>Rounding radius for the top-left corner.</summary>
    public float TopLeft;
    /// <summary>Rounding radius for the top-right corner.</summary>
    public float TopRight;
    /// <summary>Rounding radius for the bottom-left corner.</summary>
    public float BottomLeft;
    /// <summary>Rounding radius for the bottom-right corner.</summary>
    public float BottomRight;
}

/// <summary>
/// Provides configuration settings for rendering text elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct TextElementConfig
{
    /// <summary>Custom pointer to attach user-defined metadata to this text node.</summary>
    public void* UserData;

    /// <summary>The color used to paint the text glyphs.</summary>
    public Color TextColor;

    /// <summary>The unique ID of the font family registered with the renderer.</summary>
    public ushort FontId;

    /// <summary>The font size in pixels.</summary>
    public ushort FontSize;

    /// <summary>Spacing distance between consecutive letters.</summary>
    public ushort LetterSpacing;

    /// <summary>The height of each layout line boundary.</summary>
    public ushort LineHeight;

    /// <summary>Text wrap settings.</summary>
    public TextElementConfigWrapMode WrapMode;

    /// <summary>Horizontal text alignment properties.</summary>
    public TextAlignment TextAlignment;
}

/// <summary>
/// Imposes aspect ratio constraints on a UI element.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct AspectRatioElementConfig
{
    /// <summary>
    /// Target aspect ratio (Width divided by Height, e.g. 1.777f for 16:9).
    /// </summary>
    public float AspectRatio;
}

/// <summary>
/// Holds image element render configurations.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct ImageElementConfig
{
    /// <summary>
    /// Raw unmanaged pointer to source image data (texture handles, raster buffers, etc.).
    /// </summary>
    public void* ImageData;
}

/// <summary>
/// Defines target attachment points for floating elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct FloatingAttachPoints
{
    /// <summary>The attachment anchor point chosen on the floating element itself.</summary>
    public FloatingAttachPointType Element;

    /// <summary>The corresponding anchor point on the target host parent element.</summary>
    public FloatingAttachPointType Parent;
}

/// <summary>
/// Controls positioning, depth, and attachment behavior of floating (absolute positioned) UI elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct FloatingElementConfig
{
    /// <summary>Offset distance in pixels applied after anchoring calculation.</summary>
    public Vector2 Offset;

    /// <summary>Additional dimensions in pixels to expand the element's bounding size.</summary>
    public Dimensions Expand;

    /// <summary>The unique ID of the element to anchor to, if attaching to an element by ID.</summary>
    public uint ParentId;

    /// <summary>Layer drawing order index. Higher values render on top of lower values.</summary>
    public short ZIndex;

    /// <summary>The attachment point mappings.</summary>
    public FloatingAttachPoints AttachPoints;

    /// <summary>Input pointer capturing behavior details.</summary>
    public PointerCaptureMode PointerCaptureMode;

    /// <summary>Target host object type to attach the floating element to.</summary>
    public FloatingAttachToElement AttachTo;

    /// <summary>Specifies if drawing coordinates are clipped by the target container.</summary>
    public FloatingClipToElement ClipTo;
}

/// <summary>
/// Custom element layout configuration block.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CustomElementConfig
{
    /// <summary>
    /// Custom unmanaged user data pointer representing specialized drawing commands or custom layouts.
    /// </summary>
    public void* CustomData;
}

/// <summary>
/// Configures boundary clipping properties for layout nodes.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct ClipElementConfig
{
    /// <summary>If true, clips child elements horizontally when they exceed width boundaries.</summary>
    public bool Horizontal;

    /// <summary>If true, clips child elements vertically when they exceed height boundaries.</summary>
    public bool Vertical;

    /// <summary>Visual scroll offset applied to child elements inside the clip container.</summary>
    public Vector2 ChildOffset;
}

/// <summary>
/// Specifies border stroke widths for each edge of a container.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct BorderWidth
{
    /// <summary>Stroke thickness for the left border.</summary>
    public ushort Left;
    /// <summary>Stroke thickness for the right border.</summary>
    public ushort Right;
    /// <summary>Stroke thickness for the top border.</summary>
    public ushort Top;
    /// <summary>Stroke thickness for the bottom border.</summary>
    public ushort Bottom;
    /// <summary>Stroke thickness for divider lines between siblings within a container.</summary>
    public ushort BetweenChildren;
}

/// <summary>
/// Configures styling properties for drawing borders around layout nodes.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct BorderElementConfig
{
    /// <summary>The border stroke color.</summary>
    public Color Color;

    /// <summary>Border edge thickness specifications.</summary>
    public BorderWidth Width;
}

/// <summary>
/// Holds unique identifiers generated by Clay to track element lifecycle, focus, and layout state.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct ElementId
{
    /// <summary>Calculated unique integer ID.</summary>
    public uint Id;
    /// <summary>Offset factor used during looped generation.</summary>
    public uint Offset;
    /// <summary>The base hash ID representing parent contexts.</summary>
    public uint BaseId;
    /// <summary>The original source string identifier.</summary>
    public ClayString StringId;
}

/// <summary>
/// Composite declaration structure encapsulating all layout, style, and rendering configs for a single UI element.
/// Pass this to <see cref="ClayNative.ConfigureOpenElement"/> to apply styling.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct ElementDeclaration
{
    /// <summary>The unique ID handle for this element.</summary>
    public ElementId Id;

    /// <summary>General layout settings (sizing, alignment, padding).</summary>
    public LayoutConfig Layout;

    /// <summary>The element's solid background fill color.</summary>
    public Color BackgroundColor;

    /// <summary>Corner rounding radii applied to backgrounds and borders.</summary>
    public CornerRadius CornerRadius;

    /// <summary>Optional aspect ratio constraint.</summary>
    public AspectRatioElementConfig AspectRatio;

    /// <summary>Optional image settings.</summary>
    public ImageElementConfig Image;

    /// <summary>Optional floating/absolute placement settings.</summary>
    public FloatingElementConfig Floating;

    /// <summary>Optional custom drawing settings.</summary>
    public CustomElementConfig Custom;

    /// <summary>Optional scroll/overflow clipping settings.</summary>
    public ClipElementConfig Clip;

    /// <summary>Optional border stroke settings.</summary>
    public BorderElementConfig Border;

    /// <summary>Optional custom unmanaged user metadata pointer.</summary>
    public void* UserData;
}

/// <summary>
/// Describes text content and style attributes emitted inside text render commands.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct TextRenderData
{
    /// <summary>The slice of string characters to paint.</summary>
    public ClayStringSlice StringContents;

    /// <summary>The text paint color.</summary>
    public Color TextColor;

    /// <summary>The target font ID registered with the renderer.</summary>
    public ushort FontId;

    /// <summary>Font size in pixels.</summary>
    public ushort FontSize;

    /// <summary>Letter spacing value.</summary>
    public ushort LetterSpacing;

    /// <summary>Line height value.</summary>
    public ushort LineHeight;
}

/// <summary>
/// Describes a filled background rectangle emitted inside rectangle render commands.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct RectangleRenderData
{
    /// <summary>Rectangle fill color.</summary>
    public Color BackgroundColor;

    /// <summary>Corner rounding radii.</summary>
    public CornerRadius CornerRadius;
}

/// <summary>
/// Describes image content emitted inside image render commands.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct ImageRenderData
{
    /// <summary>Base background color drawn behind the image.</summary>
    public Color BackgroundColor;

    /// <summary>Corner rounding radii applied to the image bounds.</summary>
    public CornerRadius CornerRadius;

    /// <summary>Raw unmanaged pointer containing native texture or image resources.</summary>
    public void* ImageData;
}

/// <summary>
/// Describes a custom element payload emitted inside custom render commands.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CustomRenderData
{
    /// <summary>Solid background color applied to custom bounds.</summary>
    public Color BackgroundColor;

    /// <summary>Corner rounding details.</summary>
    public CornerRadius CornerRadius;

    /// <summary>Raw unmanaged pointer to custom user payload data.</summary>
    public void* CustomData;
}

/// <summary>
/// Describes clipping parameters emitted inside scissor/clip render commands.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct ClipRenderData
{
    /// <summary>True if horizontal clipping is enabled.</summary>
    public bool Horizontal;

    /// <summary>True if vertical clipping is enabled.</summary>
    public bool Vertical;
}

/// <summary>
/// Describes calculated border coordinates and stroke styling emitted inside border render commands.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct BorderRenderData
{
    /// <summary>Border line stroke color.</summary>
    public Color Color;

    /// <summary>Corner rounding details.</summary>
    public CornerRadius CornerRadius;

    /// <summary>Border edge stroke thicknesses.</summary>
    public BorderWidth Width;
}

/// <summary>
/// Emulates an anonymous C union packing specific layout drawing payloads.
/// Only one field should be accessed corresponding to the <see cref="RenderCommand.CommandType"/>.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public partial struct RenderData
{
    /// <summary>Payload for rectangle draws. Valid if command type is Rectangle.</summary>
    [FieldOffset(0)] public RectangleRenderData Rectangle;

    /// <summary>Payload for text draws. Valid if command type is Text.</summary>
    [FieldOffset(0)] public TextRenderData Text;

    /// <summary>Payload for image draws. Valid if command type is Image.</summary>
    [FieldOffset(0)] public ImageRenderData Image;

    /// <summary>Payload for custom draws. Valid if command type is Custom.</summary>
    [FieldOffset(0)] public CustomRenderData Custom;

    /// <summary>Payload for border draws. Valid if command type is Border.</summary>
    [FieldOffset(0)] public BorderRenderData Border;

    /// <summary>Payload for clip boundaries. Valid if command type is ScissorStart.</summary>
    [FieldOffset(0)] public ClipRenderData Clip;
}

/// <summary>
/// Encapsulates a single drawing instruction emitted by Clay's layout resolver.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct RenderCommand
{
    /// <summary>The absolute calculated screen bounds for this drawing command.</summary>
    public BoundingBox BoundingBox;

    /// <summary>The specific rendering payload (rectangle, text, border, etc.).</summary>
    public RenderData RenderData;

    /// <summary>Optional user-defined unmanaged metadata attached to the originating layout element.</summary>
    public void* UserData;

    /// <summary>The unique ID of the originating UI element.</summary>
    public uint Id;

    /// <summary>Layer depth index. Elements with higher Z-indexes should render on top.</summary>
    public short ZIndex;

    /// <summary>The type of rendering operation (determines which field in <see cref="RenderData"/> to access).</summary>
    public RenderCommandType CommandType;
}

/// <summary>
/// Contiguous unmanaged array containing the list of <see cref="RenderCommand"/> elements computed for the current frame.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct RenderCommandArray
{
    /// <summary>The maximum capacity allocated for the internal array.</summary>
    public int Capacity;

    /// <summary>The active number of valid drawing commands inside the list.</summary>
    public int Length;

    /// <summary>A raw unmanaged pointer to the contiguous array of <see cref="RenderCommand"/> elements.</summary>
    public RenderCommand* InternalArray;
}

/// <summary>
/// Structure containing detailed diagnostic information returned when layout errors occur.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct ErrorData
{
    /// <summary>The category of the layout engine error.</summary>
    public ErrorType ErrorType;

    /// <summary>A diagnostic text message explaining the cause of the error.</summary>
    public ClayString ErrorText;

    /// <summary>A raw user data pointer supplied during callback registration.</summary>
    public unsafe void* UserData;
}

/// <summary>
/// Holds callback function pointer registrations for native error handling.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct ErrorHandler
{
    /// <summary>A function pointer matching the error handler callback signature.</summary>
    public IntPtr ErrorHandlerFunction;

    /// <summary>Custom unmanaged user data supplied to the callback.</summary>
    public void* UserData;
}

/// <summary>
/// Unmanaged array wrapper for <see cref="ElementId"/> elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct ElementIdArray
{
    /// <summary>The capacity of the array.</summary>
    public int Capacity;
    /// <summary>The active length of the array.</summary>
    public int Length;
    /// <summary>A raw pointer to the unmanaged array of <see cref="ElementId"/> items.</summary>
    public ElementId* InternalArray;
}

/// <summary>
/// Unmanaged array wrapper for boolean elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct boolArray
{
    /// <summary>The capacity of the array.</summary>
    public int Capacity;
    /// <summary>The active length of the array.</summary>
    public int Length;
    /// <summary>A raw pointer to the unmanaged array of booleans.</summary>
    public bool* InternalArray;
}

/// <summary>
/// Unmanaged array wrapper for 32-bit signed integers.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct int32_tArray
{
    /// <summary>The capacity of the array.</summary>
    public int Capacity;
    /// <summary>The active length of the array.</summary>
    public int Length;
    /// <summary>A raw pointer to the unmanaged array of integers.</summary>
    public int* InternalArray;
}

/// <summary>
/// Unmanaged array wrapper for raw character bytes.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct charArray
{
    /// <summary>The capacity of the array.</summary>
    public int Capacity;
    /// <summary>The active length of the array.</summary>
    public int Length;
    /// <summary>A raw pointer to the unmanaged array of characters.</summary>
    public byte* InternalArray;
}

/// <summary>
/// Unmanaged array wrapper for <see cref="LayoutConfig"/> elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct LayoutConfigArray
{
    /// <summary>The capacity of the array.</summary>
    public int Capacity;
    /// <summary>The active length of the array.</summary>
    public int Length;
    /// <summary>A raw pointer to the unmanaged array of configs.</summary>
    public LayoutConfig* InternalArray;
}

/// <summary>
/// Unmanaged array wrapper for <see cref="TextElementConfig"/> elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct TextElementConfigArray
{
    /// <summary>The capacity of the array.</summary>
    public int Capacity;
    /// <summary>The active length of the array.</summary>
    public int Length;
    /// <summary>A raw pointer to the unmanaged array of text configs.</summary>
    public TextElementConfig* InternalArray;
}

/// <summary>
/// Unmanaged array wrapper for <see cref="AspectRatioElementConfig"/> elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct AspectRatioElementConfigArray
{
    /// <summary>The capacity of the array.</summary>
    public int Capacity;
    /// <summary>The active length of the array.</summary>
    public int Length;
    /// <summary>A raw pointer to the unmanaged array of aspect ratio configs.</summary>
    public AspectRatioElementConfig* InternalArray;
}

/// <summary>
/// Unmanaged array wrapper for <see cref="ImageElementConfig"/> elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct ImageElementConfigArray
{
    /// <summary>The capacity of the array.</summary>
    public int Capacity;
    /// <summary>The active length of the array.</summary>
    public int Length;
    /// <summary>A raw pointer to the unmanaged array of image configs.</summary>
    public ImageElementConfig* InternalArray;
}

/// <summary>
/// Unmanaged array wrapper for <see cref="FloatingElementConfig"/> elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct FloatingElementConfigArray
{
    /// <summary>The capacity of the array.</summary>
    public int Capacity;
    /// <summary>The active length of the array.</summary>
    public int Length;
    /// <summary>A raw pointer to the unmanaged array of floating configs.</summary>
    public FloatingElementConfig* InternalArray;
}

/// <summary>
/// Unmanaged array wrapper for <see cref="CustomElementConfig"/> elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct CustomElementConfigArray
{
    /// <summary>The capacity of the array.</summary>
    public int Capacity;
    /// <summary>The active length of the array.</summary>
    public int Length;
    /// <summary>A raw pointer to the unmanaged array of custom configs.</summary>
    public CustomElementConfig* InternalArray;
}

/// <summary>
/// Unmanaged array wrapper for <see cref="ClipElementConfig"/> elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct ClipElementConfigArray
{
    /// <summary>The capacity of the array.</summary>
    public int Capacity;
    /// <summary>The active length of the array.</summary>
    public int Length;
    /// <summary>A raw pointer to the unmanaged array of clip configs.</summary>
    public ClipElementConfig* InternalArray;
}

/// <summary>
/// Unmanaged array wrapper for <see cref="BorderElementConfig"/> elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct BorderElementConfigArray
{
    /// <summary>The capacity of the array.</summary>
    public int Capacity;
    /// <summary>The active length of the array.</summary>
    public int Length;
    /// <summary>A raw pointer to the unmanaged array of border configs.</summary>
    public BorderElementConfig* InternalArray;
}

/// <summary>
/// Unmanaged array wrapper for <see cref="ClayString"/> elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct StringArray
{
    /// <summary>The capacity of the array.</summary>
    public int Capacity;
    /// <summary>The active length of the array.</summary>
    public int Length;
    /// <summary>A raw pointer to the unmanaged array of strings.</summary>
    public ClayString* InternalArray;
}

/// <summary>
/// Unmanaged array wrapper for <see cref="SharedElementConfig"/> elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct SharedElementConfigArray
{
    /// <summary>The capacity of the array.</summary>
    public int Capacity;
    /// <summary>The active length of the array.</summary>
    public int Length;
    /// <summary>A raw pointer to the unmanaged array of shared configs.</summary>
    public SharedElementConfig* InternalArray;
}

/// <summary>
/// Represents a slice segment pointing into a native unmanaged array of <see cref="ElementConfig"/> entries.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct ElementConfigArraySlice
{
    /// <summary>The active number of configurations inside the segment.</summary>
    public int Length;

    /// <summary>A raw unmanaged pointer to the array segment of <see cref="ElementConfig"/> structures.</summary>
    public ElementConfig* InternalArray;
}

/// <summary>
/// Represents a slice segment pointing into an unmanaged array of wrapped text lines.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct WrappedTextLineArraySlice
{
    /// <summary>The active number of wrapped lines inside the segment.</summary>
    public int Length;

    /// <summary>A raw unmanaged pointer to the array segment of <see cref="WrappedTextLine"/> structures.</summary>
    public WrappedTextLine* InternalArray;
}

/// <summary>
/// Represents style settings that are shared across multiple layout nodes.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct SharedElementConfig
{
    /// <summary>Shared solid background fill color.</summary>
    public Color BackgroundColor;

    /// <summary>Shared corner rounding details.</summary>
    public CornerRadius CornerRadius;

    /// <summary>Shared custom user-defined data pointer.</summary>
    public IntPtr UserData;
}

/// <summary>
/// Associates a layout configuration style with its category type identifier.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct ElementConfig
{
    /// <summary>The style category type identifier.</summary>
    public ElementConfigType Type;

    /// <summary>The explicit union overlay structure containing the config style pointer.</summary>
    public ElementConfigUnion Config;
}

/// <summary>
/// Describes a single computed line of text resulting from word wrapping operations.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct WrappedTextLine
{
    /// <summary>The calculated width and height dimensions of this text line.</summary>
    public Dimensions Dimensions;

    /// <summary>The specific string content representing this line.</summary>
    public ClayString Line;
}

/// <summary>
/// Holds layout and wrapping details computed specifically for text elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct TextElementData
{
    /// <summary>The original source string.</summary>
    public ClayString Text;

    /// <summary>The ideal width and height of the text block prior to wrapping.</summary>
    public Dimensions PreferredDimensions;

    /// <summary>The index of the text node within the structural layout hierarchy.</summary>
    public int ElementIndex;

    /// <summary>An array slice containing details for each individual wrapped line of text.</summary>
    public WrappedTextLineArraySlice WrappedLines;
}

/// <summary>
/// Represents a core layout node within Clay's internal structural layout tree.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct LayoutElement
{
    /// <summary>An explicit C-style union containing either list of child element indices or text element details.</summary>
    public LayoutElementChildren ChildrenOrTextContent;

    /// <summary>The calculated final width and height dimensions of the element.</summary>
    public Dimensions Dimensions;

    /// <summary>The computed minimum dimensions allowed for the element.</summary>
    public Dimensions MinDimensions;

    /// <summary>A raw pointer to the <see cref="LayoutConfig"/> styling rules applied to this node.</summary>
    public unsafe LayoutConfig* LayoutConfig;

    /// <summary>An array slice containing all custom configurations attached to this node.</summary>
    public ElementConfigArraySlice ElementConfigs;

    /// <summary>The unique integer identifier of this layout element.</summary>
    public uint Id;
}

/// <summary>
/// Internal layout engine scroll tracker holding physics and position metadata for a scroll container.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct ScrollContainerDataInternal
{
    /// <summary>A raw pointer to the host layout element.</summary>
    public LayoutElement* LayoutElement;

    /// <summary>The bounding coordinates of the scroll container viewport.</summary>
    public BoundingBox BoundingBox;

    /// <summary>The total calculated width and height of the interior content being scrolled.</summary>
    public Dimensions ContentSize;

    /// <summary>Initial origin coordinate when scrolling initiated.</summary>
    public Vector2 ScrollOrigin;

    /// <summary>Pointer coordinate when scrolling initiated.</summary>
    public Vector2 PointerOrigin;

    /// <summary>Current scrolling velocity physics factor.</summary>
    public Vector2 ScrollMomentum;

    /// <summary>The active scroll position coordinates.</summary>
    public Vector2 ScrollPosition;

    /// <summary>The previous frame's offset delta.</summary>
    public Vector2 PreviousDelta;

    /// <summary>Time counter used during scroll acceleration decay.</summary>
    public float MomentumTime;

    /// <summary>The unique element ID of the scroll container.</summary>
    public uint ElementId;

    /// <summary>True if the scroll container scope was opened on this frame.</summary>
    public bool OpenThisFrame;

    /// <summary>True if scrolling is actively being manipulated by user pointer dragging.</summary>
    public bool PointerScrollActive;
}

/// <summary>
/// Structural debugging attributes associated with layout elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct DebugElementData
{
    /// <summary>True if a pointer collision was detected.</summary>
    public bool Collision;

    /// <summary>True if the element subtree is collapsed inside debugger displays.</summary>
    public bool Collapsed;
}

/// <summary>
/// Represents an entry in Clay's internal layout spatial hash map, used for fast lookup, hover, and layout calculation.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct LayoutElementHashMapItem
{
    /// <summary>Calculated absolute screen coordinates.</summary>
    public BoundingBox BoundingBox;

    /// <summary>The unique element ID.</summary>
    public ElementId ElementId;

    /// <summary>A raw pointer to the matching layout node.</summary>
    public LayoutElement* LayoutElement;

    /// <summary>Function pointer to the on-hover callback handler registered for this node.</summary>
    public IntPtr OnHoverFunction;

    /// <summary>User data pointer supplied to the on-hover handler.</summary>
    public IntPtr HoverFunctionUserData;

    /// <summary>Index tracking pointer for spatial chaining.</summary>
    public int NextIndex;

    /// <summary>Generation tick index used for lifetime recycling.</summary>
    public uint Generation;

    /// <summary>Alias identifier for alternate ID matching.</summary>
    public uint IdAlias;

    /// <summary>A raw pointer to optional debugging metadata.</summary>
    public DebugElementData* DebugData;
}

/// <summary>
/// Holds sizing metadata for a single word computed during text layout wrapping.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct MeasuredWord
{
    /// <summary>The character index offset where the word begins inside the base string.</summary>
    public int StartOffset;

    /// <summary>The length of the word in bytes.</summary>
    public int Length;

    /// <summary>The calculated rendering width of the word in pixels.</summary>
    public float Width;

    /// <summary>The index pointer to the next consecutive word.</summary>
    public int Next;
}

/// <summary>
/// Represents an item stored inside Clay's internal text measurement cache.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct MeasureTextCacheItem
{
    /// <summary>The raw width and height dimensions of the text prior to wrapping.</summary>
    public Dimensions UnwrappedDimensions;

    /// <summary>Index tracking where the word sizing list starts inside global tables.</summary>
    public int MeasuredWordsStartIndex;

    /// <summary>The minimum width allowed for wrapping before text overlaps.</summary>
    public float MinWidth;

    /// <summary>True if the string contains explicit newline characters.</summary>
    public bool ContainsNewlines;

    /// <summary>The hash ID associated with this cached layout.</summary>
    public uint Id;

    /// <summary>Index pointer for cache collision chaining.</summary>
    public int NextIndex;

    /// <summary>Generation tick index used for cache expiration and reuse.</summary>
    public uint Generation;
}

/// <summary>
/// A layout tree node structure tracking hierarchical rendering positioning.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct LayoutElementTreeNode
{
    /// <summary>A raw pointer to the layout element.</summary>
    public LayoutElement* LayoutElement;

    /// <summary>The computed absolute coordinate position.</summary>
    public Vector2 Position;

    /// <summary>Spacing coordinate offset applied to the next sequential child node.</summary>
    public Vector2 NextChildOffset;
}

/// <summary>
/// Defines the entry root configurations for a layout tree rendering hierarchy.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct LayoutElementTreeRoot
{
    /// <summary>The index pointer of the layout element acting as subtree root.</summary>
    public int LayoutElementIndex;

    /// <summary>The unique ID of the parent element hosting this tree.</summary>
    public uint ParentId;

    /// <summary>The unique ID of the active clipping boundaries.</summary>
    public uint ClipElementId;

    /// <summary>Layer drawing order index.</summary>
    public short ZIndex;

    /// <summary>Active coordinate offset of the pointer relative to root.</summary>
    public Vector2 PointerOffset;
}

/// <summary>
/// Public structure containing scroll coordinates and dimensions for scrollable elements.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct ScrollContainerData
{
    /// <summary>A raw pointer to the active scroll position vector.</summary>
    public unsafe Vector2* ScrollPosition;

    /// <summary>The visible viewport bounds of the scroll container.</summary>
    public Dimensions ScrollContainerDimensions;

    /// <summary>The total dimensions of the scrolled interior content.</summary>
    public Dimensions ContentDimensions;

    /// <summary>The boundary clipping configurations applied.</summary>
    public ClipElementConfig Config;

    /// <summary>True if the scroll container element was found inside layout trees.</summary>
    public bool Found;
}

/// <summary>
/// Public structure containing calculated layout coordinates.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct ElementData
{
    /// <summary>The computed absolute bounding coordinates.</summary>
    public BoundingBox BoundingBox;

    /// <summary>True if the element was successfully located inside active layout structures.</summary>
    public bool Found;
}

/// <summary>
/// Structure packing diagnostic warning messaging.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct Warning
{
    /// <summary>The base diagnostic code message.</summary>
    public ClayString BaseMessage;

    /// <summary>The dynamic detail description.</summary>
    public ClayString DynamicMessage;
}

/// <summary>
/// Emulates an anonymous C-style union containing either list of child element indices or text element details.
/// Field offsets overlap so only one field should be accessed corresponding to layout node type.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public unsafe partial struct LayoutElementChildren
{
    /// <summary>The list array tracking sibling elements. Valid if node is a layout container.</summary>
    [FieldOffset(0)] public LayoutElementChildrenData Children;

    /// <summary>A raw pointer to the text styling metadata. Valid if node is a text element.</summary>
    [FieldOffset(0)] public TextElementData* TextElementData;
}

/// <summary>
/// Holds list of child elements inside a container.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct LayoutElementChildrenData
{
    /// <summary>A raw pointer to the unmanaged array of child layout element indices.</summary>
    public int* Elements;

    /// <summary>The active number of child elements.</summary>
    public ushort Length;
}

/// <summary>
/// Emulates an anonymous configuration C union inside <see cref="ElementConfig"/>, holding style detail pointers.
/// Only one field should be accessed corresponding to the <see cref="ElementConfig.Type"/>.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public unsafe partial struct ElementConfigUnion
{
    /// <summary>Text styling config pointer. Valid if type is Text.</summary>
    [FieldOffset(0)] public TextElementConfig* TextElementConfig;

    /// <summary>Aspect ratio constraint config pointer. Valid if type is Aspect.</summary>
    [FieldOffset(0)] public AspectRatioElementConfig* AspectRatioElementConfig;

    /// <summary>Image config pointer. Valid if type is Image.</summary>
    [FieldOffset(0)] public ImageElementConfig* ImageElementConfig;

    /// <summary>Floating coordinate config pointer. Valid if type is Floating.</summary>
    [FieldOffset(0)] public FloatingElementConfig* FloatingElementConfig;

    /// <summary>Custom renderer config pointer. Valid if type is Custom.</summary>
    [FieldOffset(0)] public CustomElementConfig* CustomElementConfig;

    /// <summary>Clipping config pointer. Valid if type is Clip.</summary>
    [FieldOffset(0)] public ClipElementConfig* ClipElementConfig;

    /// <summary>Border config pointer. Valid if type is Border.</summary>
    [FieldOffset(0)] public BorderElementConfig* BorderElementConfig;

    /// <summary>Shared config style pointer. Valid if type is Shared.</summary>
    [FieldOffset(0)] public SharedElementConfig* SharedElementConfig;
}
