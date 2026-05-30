# Getting Started with ClaySharp

This guide walks you through the step-by-step process of integrating **ClaySharp** into your C# rendering pipeline. 

---

## 📋 Integration Steps

### Step 1: Memory Arena Allocation
Clay runs within a single block of unmanaged memory (a linear arena). You must query the engine for its minimum allocation size, allocate a backing block, and initialize the `Arena` structure:

```csharp
using System.Runtime.InteropServices;
using ClaySharp.Interop;

// 1. Query minimum size required by the layout engine
uint minMemoryBytes = ClayNative.MinMemorySize();

// 2. Allocate the unmanaged backing memory block
void* arenaMemory = (void*)Marshal.AllocHGlobal((int)minMemoryBytes);

// 3. Create the Arena structure
Arena arena = ClayNative.CreateArenaWithCapacityAndMemory((UIntPtr)minMemoryBytes, arenaMemory);
```

---

### Step 2: Initialize Context
Initialize Clay with the memory arena, the initial screen dimensions, and a diagnostic error handler:

```csharp
var dimensions = new Dimensions { Width = 1024, Height = 768 };

var errorHandler = new ErrorHandler
{
    ErrorHandlerFunction = Marshal.GetFunctionPointerForDelegate(new Action<ErrorData>(OnClayError)),
    UserData = null
};

// Initialize the native layout context
ClayNative.Clay_Initialize(arena, dimensions, errorHandler);

// ---

static void OnClayError(ErrorData error)
{
    string msg = Marshal.PtrToStringAnsi((IntPtr)error.ErrorText.Chars, error.ErrorText.Length);
    Console.WriteLine($"[Clay Layout Error] {error.ErrorType}: {msg}");
}
```

---

### Step 3: Register Text Measurement Callback
If your user interface displays text elements, Clay must be able to calculate text dimensions. Register a callback that implements your custom text measurement algorithm:

```csharp
// Define the measurement delegate
var measureCallback = new ClayNative.Clay_MeasureTextCallback(MeasureText);

// Register the callback with Clay
ClayNative.SetMeasureTextFunction(Marshal.GetFunctionPointerForDelegate(measureCallback), null);

// ---

static Dimensions MeasureText(ClayStringSlice text, TextElementConfig* config, void* userData)
{
    // Access styling using config->FontSize, config->FontId, etc.
    float width = text.Length * (config->FontSize * 0.5f); // Simple fallback width approximation
    return new Dimensions { Width = width, Height = config->FontSize };
}
```

---

### Step 4: The Frame Layout Loop
During your application update and render frames, feed pointer inputs and execute a layout calculations block. You can declare layout elements using C# resource scopes (`using` blocks):

```csharp
public static unsafe void RunLayoutFrame(float mouseX, float mouseY, bool isLeftMouseDown)
{
    // A. Feed pointer position and button state to Clay
    ClayNative.SetPointerState(new Vector2 { X = mouseX, Y = mouseY }, isLeftMouseDown);

    // B. Begin layout pass calculation
    ClayNative.BeginLayout();

    // Declare a root panel declaration
    var rootPanel = new ElementDeclaration
    {
        Id = ClayNative.GetElementId(CreateClayString("root_panel")),
        BackgroundColor = new Color { R = 20, G = 20, B = 22, A = 255 },
        Layout = new LayoutConfig
        {
            LayoutDirection = LayoutDirection.LeftToRight,
            ChildAlignment = new ChildAlignment { X = LayoutAlignmentX.Center, Y = LayoutAlignmentY.Center },
            Sizing = new Sizing
            {
                Width = new SizingAxis { Type = SizingType.Grow },
                Height = new SizingAxis { Type = SizingType.Grow }
            }
        }
    };

    // C. Declare the UI structure
    using (new ClayElementScope(in rootPanel))
    {
        // Define children here...
    }

    // D. Finalize calculations and get the output draw commands
    RenderCommandArray commands = ClayNative.EndLayout();

    // E. Paint the UI using the commands
    RenderCommands(commands);
}
```

---

### Step 5: Processing Render Commands
Iterate through the resulting `RenderCommandArray` and draw each item using your graphics backend (e.g. SkiaSharp, OpenGL, Vulkan):

```csharp
static void RenderCommands(RenderCommandArray commands)
{
    for (int i = 0; i < commands.Length; i++)
    {
        RenderCommand* cmd = ClayNative.RenderCommandArray_Get(commands, i);
        if (cmd == null) continue;

        switch (cmd->CommandType)
        {
            case RenderCommandType.Rectangle:
                // Draw filled rectangle using cmd->BoundingBox and cmd->RenderData.Rectangle
                break;

            case RenderCommandType.Text:
                // Paint text using cmd->BoundingBox and cmd->RenderData.Text
                break;

            case RenderCommandType.ScissorStart:
                // Clip drawing coordinates to cmd->BoundingBox area
                break;

            case RenderCommandType.ScissorEnd:
                // Restore previous clipping boundaries
                break;
        }
    }
}
```