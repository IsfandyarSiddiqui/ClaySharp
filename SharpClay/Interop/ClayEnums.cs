namespace SharpClay.Interop;

/// <summary>
/// Specifies the type of layout configuration applied to a UI element.
/// This configuration determines which rendering, sizing, or layout rules Clay applies to the element.
/// </summary>
public enum ElementConfigType : byte
{
    /// <summary>No configuration type is set.</summary>
    None = 0,
    /// <summary>Applies a border layout config, drawing borders on one or more edges.</summary>
    Border = 1,
    /// <summary>Allows the element to float dynamically relative to other elements (absolute positioning).</summary>
    Floating = 2,
    /// <summary>Clips child elements when they exceed the boundaries of this element.</summary>
    Clip = 3,
    /// <summary>Enforces a specific aspect ratio on the element.</summary>
    Aspect = 4,
    /// <summary>Specifies that the element renders an image.</summary>
    Image = 5,
    /// <summary>Specifies that the element renders text.</summary>
    Text = 6,
    /// <summary>Applies a custom layout configuration or renderer for custom elements.</summary>
    Custom = 7,
    /// <summary>Uses a shared style or configuration block.</summary>
    Shared = 8
}

/// <summary>
/// Defines the layout direction along which children are arranged.
/// </summary>
public enum LayoutDirection : byte
{
    /// <summary>Arranges child elements horizontally from left to right.</summary>
    LeftToRight = 0,
    /// <summary>Arranges child elements vertically from top to bottom.</summary>
    TopToBottom = 1
}

/// <summary>
/// Controls horizontal alignment of children within a layout container.
/// </summary>
public enum LayoutAlignmentX : byte
{
    /// <summary>Aligns children to the left edge of the container.</summary>
    Left = 0,
    /// <summary>Aligns children to the right edge of the container.</summary>
    Right = 1,
    /// <summary>Centers children horizontally within the container.</summary>
    Center = 2
}

/// <summary>
/// Controls vertical alignment of children within a layout container.
/// </summary>
public enum LayoutAlignmentY : byte
{
    /// <summary>Aligns children to the top edge of the container.</summary>
    Top = 0,
    /// <summary>Aligns children to the bottom edge of the container.</summary>
    Bottom = 1,
    /// <summary>Centers children vertically within the container.</summary>
    Center = 2
}

/// <summary>
/// Determines how an element is sized along a layout axis.
/// </summary>
public enum SizingType : byte
{
    /// <summary>Sizes the element to exactly fit its content.</summary>
    Fit = 0,
    /// <summary>Allows the element to grow and fill remaining space in its parent container.</summary>
    Grow = 1,
    /// <summary>Sizes the element as a percentage of its parent container's size.</summary>
    Percent = 2,
    /// <summary>Sizes the element to a fixed pixel dimension.</summary>
    Fixed = 3
}

/// <summary>
/// Represents the interaction state of a pointer (e.g. mouse or touch) on a UI element.
/// </summary>
public enum PointerDataInteractionState : byte
{
    /// <summary>The pointer was pressed down on this frame.</summary>
    PressedThisFrame = 0,
    /// <summary>The pointer is currently held down.</summary>
    Pressed = 1,
    /// <summary>The pointer was released on this frame.</summary>
    ReleasedThisFrame = 2,
    /// <summary>The pointer is currently released/up.</summary>
    Released = 3
}

/// <summary>
/// Defines how text within a text element wraps.
/// </summary>
public enum TextElementConfigWrapMode : byte
{
    /// <summary>Wraps text onto new lines at word boundaries.</summary>
    Words = 0,
    /// <summary>Only wraps text when explicit newline characters (\n) are encountered.</summary>
    Newlines = 1,
    /// <summary>Disables text wrapping entirely; text will overflow.</summary>
    None = 2
}

/// <summary>
/// Specifies the horizontal alignment of text within its bounding container.
/// </summary>
public enum TextAlignment : byte
{
    /// <summary>Aligns text to the left margin.</summary>
    Left = 0,
    /// <summary>Centers text horizontally.</summary>
    Center = 1,
    /// <summary>Aligns text to the right margin.</summary>
    Right = 2
}

/// <summary>
/// Defines the attachment points on an element and its target when using floating (absolute) positioning.
/// </summary>
public enum FloatingAttachPointType : byte
{
    /// <summary>Anchor at the top-left corner.</summary>
    LeftTop = 0,
    /// <summary>Anchor at the middle-left edge.</summary>
    LeftCenter = 1,
    /// <summary>Anchor at the bottom-left corner.</summary>
    LeftBottom = 2,
    /// <summary>Anchor at the top-center edge.</summary>
    CenterTop = 3,
    /// <summary>Anchor at the true center.</summary>
    CenterCenter = 4,
    /// <summary>Anchor at the bottom-center edge.</summary>
    CenterBottom = 5,
    /// <summary>Anchor at the top-right corner.</summary>
    RightTop = 6,
    /// <summary>Anchor at the middle-right edge.</summary>
    RightCenter = 7,
    /// <summary>Anchor at the bottom-right corner.</summary>
    RightBottom = 8
}

/// <summary>
/// Controls pointer capture behavior for floating elements.
/// </summary>
public enum PointerCaptureMode : byte
{
    /// <summary>Capture pointer input events when interacting with the floating element.</summary>
    Capture = 0,
}

/// <summary>
/// Identifies which element a floating container attaches to.
/// </summary>
public enum FloatingAttachToElement : byte
{
    /// <summary>Does not attach to any element (defaults to absolute layout root coordinate space).</summary>
    None = 0,
    /// <summary>Attaches to the direct parent element in the layout tree.</summary>
    Parent = 1,
    /// <summary>Attaches to an element with a specific ID.</summary>
    ElementWithId = 2,
    /// <summary>Attaches to the root layout container.</summary>
    Root = 3
}

/// <summary>
/// Controls whether a floating element should be clipped by another element's bounds.
/// </summary>
public enum FloatingClipToElement : byte
{
    /// <summary>No clipping is applied; the floating element is allowed to draw outside bounds.</summary>
    None = 0,
    /// <summary>Clips the floating element to the bounding box of its attached parent.</summary>
    AttachedParent = 1
}

/// <summary>
/// Specifies the rendering operation command type emitted in the output render command array.
/// </summary>
public enum RenderCommandType : byte
{
    /// <summary>A null command, does not render anything.</summary>
    None = 0,
    /// <summary>Draws a filled rectangle with optional corner rounding.</summary>
    Rectangle = 1,
    /// <summary>Draws a border outline around an area.</summary>
    Border = 2,
    /// <summary>Renders a run of text.</summary>
    Text = 3,
    /// <summary>Draws a source image.</summary>
    Image = 4,
    /// <summary>Enables clipping/scissor test using the command's bounding box.</summary>
    ScissorStart = 5,
    /// <summary>Disables clipping/scissor test, restoring the previous drawing area.</summary>
    ScissorEnd = 6,
    /// <summary>Executes a custom user-defined drawing operation.</summary>
    Custom = 7
}

/// <summary>
/// Defines the specific errors that can occur during layout calculation or configuration in Clay.
/// </summary>
public enum ErrorType : byte
{
    /// <summary>A text measurement callback was not supplied, but a text element was encountered.</summary>
    TextMeasurementFunctionNotProvided = 0,
    /// <summary>The internal memory arena capacity has been exceeded.</summary>
    ArenaCapacityExceeded = 1,
    /// <summary>The maximum element count capacity specified has been exceeded.</summary>
    ElementsCapacityExceeded = 2,
    /// <summary>The internal cache for storing text measurements has run out of space.</summary>
    TextMeasurementCapacityExceeded = 3,
    /// <summary>A duplicate element ID was declared in the layout tree.</summary>
    DuplicateId = 4,
    /// <summary>A floating container requested attachment to a parent ID that could not be found.</summary>
    FloatingContainerParentNotFound = 5,
    /// <summary>A layout percentage value was specified which was greater than 1.0 (100%).</summary>
    PercentageOver1 = 6,
    /// <summary>An unclassified internal engine error occurred.</summary>
    InternalError = 7
}
