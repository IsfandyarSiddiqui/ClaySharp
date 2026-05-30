using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: DisableRuntimeMarshalling]

namespace ClaySharp.Interop;

/// <summary>
/// Provides low-level, high-performance C# bindings to the native Clay layout library.
/// Uses source-generated marshalling via <see cref="LibraryImportAttribute"/> and compiles with runtime marshalling disabled
/// for zero-overhead, direct native transitions.
/// </summary>
/// <remarks>
/// Clay is a single-header C library for high-performance 2D layout. This class provides the direct interop wrapper.
/// Since runtime marshalling is disabled, all parameters and return types are marshaled at compile-time by the C# compiler.
/// Unsafe pointers are utilized heavily; developers must ensure correct lifetime management of memory backings and arenas.
/// </remarks>
public static unsafe partial class ClayNative
{
    private const string LibraryName = "clay";

    /// <summary>
    /// Retrieves the minimum size in bytes of the memory backing required for Clay's internal allocator.
    /// Call this before allocating memory for the arena.
    /// </summary>
    /// <returns>The minimum memory size in bytes.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_MinMemorySize")]
    public static partial uint MinMemorySize();

    /// <summary>
    /// Creates a memory allocation arena using a pre-allocated block of raw unmanaged memory.
    /// </summary>
    /// <param name="capacity">The capacity of the memory backing in bytes. Usually obtained via <see cref="MinMemorySize"/>.</param>
    /// <param name="memory">A raw pointer to the block of unmanaged memory allocated for Clay.</param>
    /// <returns>A initialized <see cref="Arena"/> struct ready to be passed to initialization.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_CreateArenaWithCapacityAndMemory")]
    public static partial Arena CreateArenaWithCapacityAndMemory(UIntPtr capacity, void* memory);

    /// <summary>
    /// Initializes the native Clay layout engine context with a memory arena, initial window dimensions, and an error handler.
    /// </summary>
    /// <param name="arena">The allocated memory arena for Clay's internal structures.</param>
    /// <param name="layoutDimensions">The initial width and height of the layout viewport.</param>
    /// <param name="errorHandler">The error handler callback and user data to receive error notifications.</param>
    /// <returns>A raw pointer to the initialized internal <see cref="Context"/> structure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_Initialize")]
    public static partial Context* Clay_Initialize(Arena arena, Dimensions layoutDimensions, ErrorHandler errorHandler);

    /// <summary>
    /// Gets a pointer to the active, thread-local Clay layout context.
    /// </summary>
    /// <returns>A pointer to the current <see cref="Context"/>, or null if none is active.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_GetCurrentContext")]
    [SuppressGCTransition]
    public static partial Context* GetCurrentContext();

    /// <summary>
    /// Sets the active, thread-local Clay layout context.
    /// Useful for multi-context architectures where multiple layout states are maintained.
    /// </summary>
    /// <param name="context">A pointer to the <see cref="Context"/> to set as active.</param>
    [LibraryImport(LibraryName, EntryPoint = "Clay_SetCurrentContext")]
    [SuppressGCTransition]
    public static partial void SetCurrentContext(Context* context);

    /// <summary>
    /// Begins a new layout pass. Must be called every frame before declaring UI elements.
    /// </summary>
    [LibraryImport(LibraryName, EntryPoint = "Clay_BeginLayout")]
    public static partial void BeginLayout();

    /// <summary>
    /// Concludes the current layout pass, computes alignments, sizes, and positions, and returns the resulting rendering commands.
    /// </summary>
    /// <returns>A <see cref="RenderCommandArray"/> containing the drawing operations to perform for the frame.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_EndLayout")]
    public static partial RenderCommandArray EndLayout();

    /// <summary>
    /// Updates the dimensions of the layout viewport. Usually called on window resize.
    /// </summary>
    /// <param name="dimensions">The new viewport width and height.</param>
    [LibraryImport(LibraryName, EntryPoint = "Clay_SetLayoutDimensions")]
    public static partial void SetLayoutDimensions(Dimensions dimensions);

    /// <summary>
    /// Updates Clay's pointer input state (e.g. mouse or touch pointer coordinates and button state).
    /// </summary>
    /// <param name="position">The current 2D screen coordinate of the pointer.</param>
    /// <param name="pointerDown">True if the primary pointer button is pressed; otherwise, false.</param>
    [LibraryImport(LibraryName, EntryPoint = "Clay_SetPointerState")]
    [SuppressGCTransition]
    public static partial void SetPointerState(Vector2 position, [MarshalAs(UnmanagedType.U1)] bool pointerDown);

    /// <summary>
    /// Processes scroll movements for scrollable containers based on user actions.
    /// </summary>
    /// <param name="enableDragScrolling">If true, allows users to click and drag to scroll.</param>
    /// <param name="scrollDelta">The scroll wheel or trackpad delta offset applied this frame.</param>
    /// <param name="deltaTime">The time elapsed since the last frame in seconds.</param>
    [LibraryImport(LibraryName, EntryPoint = "Clay_UpdateScrollContainers")]
    public static partial void UpdateScrollContainers([MarshalAs(UnmanagedType.U1)] bool enableDragScrolling, Vector2 scrollDelta, float deltaTime);

    /// <summary>
    /// Checks if any element in the active layout was hovered by the pointer during the current frame.
    /// </summary>
    /// <returns>True if a layout element is currently hovered; otherwise, false.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_Hovered")]
    [SuppressGCTransition]
    [return: MarshalAs(UnmanagedType.U1)]
    public static partial bool Hovered();

    /// <summary>
    /// Defines the signature of the callback invoked when a pointer hovers over an element configured with an hover handler.
    /// </summary>
    /// <param name="elementId">The unique ID of the hovered element.</param>
    /// <param name="pointerData">The position and button state of the pointer during interaction.</param>
    /// <param name="userData">A custom user-defined data pointer passed during registration.</param>
    public delegate void OnHoverCallback(ElementId elementId, PointerData pointerData, IntPtr userData);

    /// <summary>
    /// Registers a global hover callback that is invoked when any element configured to trigger on-hover events is hovered.
    /// </summary>
    /// <param name="onHoverFunction">A function pointer to a method matching the <see cref="OnHoverCallback"/> signature.</param>
    /// <param name="userData">A custom unmanaged data pointer that will be supplied to the callback.</param>
    [LibraryImport(LibraryName, EntryPoint = "Clay_OnHover")]
    public static partial void OnHover(IntPtr onHoverFunction, IntPtr userData);

    /// <summary>
    /// Determines whether the pointer is currently positioned over a specific layout element.
    /// </summary>
    /// <param name="elementId">The unique ID of the element to check.</param>
    /// <returns>True if the pointer is hovering over the specified element; otherwise, false.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_PointerOver")]
    [SuppressGCTransition]
    [return: MarshalAs(UnmanagedType.U1)]
    public static partial bool PointerOver(ElementId elementId);

    /// <summary>
    /// Retrieves an array of all layout element IDs that the pointer is currently positioned over.
    /// </summary>
    /// <returns>An array slice of <see cref="ElementId"/> structs representing the hovered hierarchy.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_GetPointerOverIds")]
    public static partial ElementIdArray GetPointerOverIds();

    /// <summary>
    /// Gets the current active scroll offset accumulated for scroll containers.
    /// </summary>
    /// <returns>A <see cref="Vector2"/> representing the horizontal and vertical scroll offsets.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_GetScrollOffset")]
    [SuppressGCTransition]
    public static partial Vector2 GetScrollOffset();

    /// <summary>
    /// Retrieves calculated layout details (such as the computed bounding box) for a specific element.
    /// </summary>
    /// <param name="id">The unique ID of the element.</param>
    /// <returns>An <see cref="ElementData"/> structure containing layout metadata.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_GetElementData")]
    public static partial ElementData GetElementData(ElementId id);

    /// <summary>
    /// Retrieves scroll offset and container dimensions for a scrollable layout element.
    /// </summary>
    /// <param name="id">The unique ID of the scroll container element.</param>
    /// <returns>A <see cref="ScrollContainerData"/> structure containing scroll state.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_GetScrollContainerData")]
    public static partial ScrollContainerData GetScrollContainerData(ElementId id);

    /// <summary>
    /// Defines the signature of the text measurement callback used by Clay to compute text dimensions.
    /// </summary>
    /// <param name="text">A slice of the string content to measure.</param>
    /// <param name="config">A pointer to the text styling configuration (font, size, line height, spacing, etc.).</param>
    /// <param name="userData">A custom user-defined data pointer passed during registration.</param>
    /// <returns>The computed width and height of the rendered text block.</returns>
    public delegate Dimensions Clay_MeasureTextCallback(ClayStringSlice text, TextElementConfig* config, void* userData);

    /// <summary>
    /// Configures the callback function that Clay will invoke whenever it needs to calculate text sizing.
    /// This callback must be provided if the layout tree contains any text elements.
    /// </summary>
    /// <param name="measureTextFunction">A function pointer matching the <see cref="Clay_MeasureTextCallback"/> signature.</param>
    /// <param name="userData">A custom data pointer that will be supplied to the measurement callback.</param>
    [LibraryImport(LibraryName, EntryPoint = "Clay_SetMeasureTextFunction")]
    public static partial void SetMeasureTextFunction(IntPtr measureTextFunction, void* userData);

    /// <summary>
    /// Sets a custom callback to query scroll offsets if external scroll integration is desired.
    /// </summary>
    /// <param name="queryScrollOffsetFunction">A function pointer to the scroll querying function.</param>
    /// <param name="userData">A custom data pointer passed to the callback.</param>
    [LibraryImport(LibraryName, EntryPoint = "Clay_SetQueryScrollOffsetFunction")]
    public static partial void SetQueryScrollOffsetFunction(IntPtr queryScrollOffsetFunction, void* userData);

    /// <summary>
    /// Resets the internal text measurement cache. Call this if fonts, sizes, or global zoom levels change.
    /// </summary>
    [LibraryImport(LibraryName, EntryPoint = "Clay_ResetMeasureTextCache")]
    public static partial void ResetMeasureTextCache();

    /// <summary>
    /// Enables or disables the native Clay debugger overlay.
    /// </summary>
    /// <param name="enabled">True to enable debug mode; otherwise, false.</param>
    [LibraryImport(LibraryName, EntryPoint = "Clay_SetDebugModeEnabled")]
    public static partial void SetDebugModeEnabled([MarshalAs(UnmanagedType.U1)] bool enabled);

    /// <summary>
    /// Checks whether Clay's debug mode is currently active.
    /// </summary>
    /// <returns>True if debugging is enabled; otherwise, false.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_IsDebugModeEnabled")]
    [SuppressGCTransition]
    [return: MarshalAs(UnmanagedType.U1)]
    public static partial bool IsDebugModeEnabled();

    /// <summary>
    /// Enables or disables viewport culling. When enabled, elements outside the visible viewport are excluded from rendering.
    /// </summary>
    /// <param name="enabled">True to enable clipping/culling; otherwise, false.</param>
    [LibraryImport(LibraryName, EntryPoint = "Clay_SetCullingEnabled")]
    public static partial void SetCullingEnabled([MarshalAs(UnmanagedType.U1)] bool enabled);

    /// <summary>
    /// Gets the maximum number of UI elements that can be stored in the layout tree.
    /// </summary>
    /// <returns>The maximum element capacity.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_GetMaxElementCount")]
    [SuppressGCTransition]
    public static partial int GetMaxElementCount();

    /// <summary>
    /// Sets the maximum number of UI elements that can be stored in the layout tree.
    /// Must be configured to match structural complexities.
    /// </summary>
    /// <param name="maxElementCount">The new maximum element capacity.</param>
    [LibraryImport(LibraryName, EntryPoint = "Clay_SetMaxElementCount")]
    public static partial void SetMaxElementCount(int maxElementCount);

    /// <summary>
    /// Gets the maximum capacity of the word cache used during text measurement.
    /// </summary>
    /// <returns>The word cache capacity.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_GetMaxMeasureTextCacheWordCount")]
    [SuppressGCTransition]
    public static partial int GetMaxMeasureTextCacheWordCount();

    /// <summary>
    /// Sets the maximum capacity of the word cache used during text measurement.
    /// </summary>
    /// <param name="maxMeasureTextCacheWordCount">The new cache word limit.</param>
    [LibraryImport(LibraryName, EntryPoint = "Clay_SetMaxMeasureTextCacheWordCount")]
    public static partial void SetMaxMeasureTextCacheWordCount(int maxMeasureTextCacheWordCount);

    /// <summary>
    /// Low-level function: opens a new layout element scope.
    /// Must be paired with a corresponding call to <see cref="CloseElement"/>.
    /// </summary>
    [LibraryImport(LibraryName, EntryPoint = "Clay__OpenElement")]
    public static partial void OpenElement();

    /// <summary>
    /// Low-level function: configures the currently open layout element with the provided styling declarations.
    /// </summary>
    /// <param name="config">The configuration parameters for the element.</param>
    [LibraryImport(LibraryName, EntryPoint = "Clay__ConfigureOpenElement")]
    public static partial void ConfigureOpenElement(ElementDeclaration config);

    /// <summary>
    /// Low-level function: configures the currently open layout element passing a reference to avoid structure copy.
    /// </summary>
    /// <param name="config">A reference to the configuration parameters.</param>
    [LibraryImport(LibraryName, EntryPoint = "Clay__ConfigureOpenElementPtr")]
    public static partial void ConfigureOpenElementPtr(in ElementDeclaration config);

    /// <summary>
    /// Low-level function: closes the current active layout element scope.
    /// </summary>
    [LibraryImport(LibraryName, EntryPoint = "Clay__CloseElement")]
    public static partial void CloseElement();

    /// <summary>
    /// Low-level function: opens and declares a text element within the current parent scope.
    /// </summary>
    /// <param name="text">The string text to display.</param>
    /// <param name="textConfig">A pointer to the text rendering style parameters.</param>
    [LibraryImport(LibraryName, EntryPoint = "Clay__OpenTextElement")]
    public static partial void OpenTextElement(ClayString text, TextElementConfig* textConfig);

    /// <summary>
    /// Low-level function: copies the text configuration into Clay's internal layout storage arena.
    /// </summary>
    /// <param name="config">The text configuration to store.</param>
    /// <returns>A pointer to the persisted config within native memory.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay__StoreTextElementConfig")]
    public static partial TextElementConfig* StoreTextElementConfig(TextElementConfig config);

    /// <summary>
    /// Retrieves the unique integer ID of the currently open parent element.
    /// </summary>
    /// <returns>The parent element's ID value.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay__GetParentElementId")]
    [SuppressGCTransition]
    public static partial uint GetParentElementId();

    /// <summary>
    /// Generates a unique layout ID based on a string key and hashing parameters.
    /// </summary>
    /// <param name="key">The source string to hash.</param>
    /// <param name="offset">An offset index to append to avoid conflicts.</param>
    /// <param name="seed">A custom hashing seed value.</param>
    /// <returns>A hashed <see cref="ElementId"/> structure.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay__HashString")]
    [SuppressGCTransition]
    public static partial ElementId HashString(ClayString key, uint offset, uint seed);

    /// <summary>
    /// Computes a unique element ID from a descriptive ID string.
    /// </summary>
    /// <param name="idString">The string key identifying the element.</param>
    /// <returns>A unique <see cref="ElementId"/> for element declarations.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_GetElementId")]
    public static partial ElementId GetElementId(ClayString idString);

    /// <summary>
    /// Computes a unique element ID from a descriptive ID string and an index (e.g. inside loops).
    /// </summary>
    /// <param name="idString">The base string key identifying the collection.</param>
    /// <param name="index">An integer loop index to guarantee uniqueness.</param>
    /// <returns>A unique indexed <see cref="ElementId"/>.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_GetElementIdWithIndex")]
    public static partial ElementId GetElementIdWithIndex(ClayString idString, uint index);

    /// <summary>
    /// Helper: retrieves a pointer to a specific <see cref="RenderCommand"/> inside a <see cref="RenderCommandArray"/> at the specified index.
    /// </summary>
    /// <param name="array">The list of render commands returned by <see cref="EndLayout"/>.</param>
    /// <param name="index">The 0-based index of the command to retrieve.</param>
    /// <returns>A pointer to the render command structure, or null if the index is out of bounds.</returns>
    [LibraryImport(LibraryName, EntryPoint = "Clay_RenderCommandArray_Get")]
    public static partial RenderCommand* RenderCommandArray_Get(RenderCommandArray array, int index);
}
