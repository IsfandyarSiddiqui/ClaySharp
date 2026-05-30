# Introduction to SharpClay

**SharpClay** is a modern C# binding for **Clay**, a high-performance 2D layout engine written as a single-header C library. Designed for game engines, desktop shells, and graphics-intensive applications, SharpClay allows C# developers to define complex, responsive user interfaces with absolute minimum overhead.

---

## 🚀 Key Features

* **Zero-Allocation Layout Loop:** Designed from the ground up to prevent heap allocation churn. Dynamic layout trees are computed inside an unmanaged memory block, bypassing C# Garbage Collection completely.
* **Flexbox-like Alignment Rules:** Easily position children horizontally or vertically with customized alignment (left/top, center, right/bottom) and layout spacing gap sizes.
* **Custom Sizing & Aspect Ratios:** Supports flexible sizing constraints including fixed sizing (`Fixed`), matching children bounds (`Fit`), proportioning sibling layouts (`Grow`), or scaling relative to parents (`Percent`), alongside explicit aspect ratio locks.
* **Floating (Absolute) Coordinates:** Design menus, tooltips, dialogs, and panels that float relative to parents, specific elements by ID, or the viewport root with dedicated depth (`ZIndex`) layers.
* **Viewport Culling & Clipping:** Enable boundary clipping and culling elements outside the visible viewport to optimize rendering performance for lists or nested views.
* **Pointer Hover & Scroll Handling:** Seamlessly update mouse/touch inputs and process viewport scrolling offsets, momentum, and click collisions natively.

---

## 🏛 Architecture & Performance Design

Unlike traditional UI frameworks that utilize deep inheritance trees, reflection, or heavy managed wrappers, SharpClay focuses on raw ABI compatibility and ultra-fast transitions:

```
┌─────────────────┐       [LibraryImport]       ┌──────────────┐
│  C# Application │ ──────────────────────────► │ Clay Native  │
│  (Managed code) │ ◄────────────────────────── │  (C library) │
└─────────────────┘      Disable Marshalling    └──────────────┘
```

1. **Source-Generated Marshalling (`[LibraryImport]`):** By shifting native marshalling code generation to compile time, C# compiles optimized transition instructions, avoiding reflection overhead entirely.
2. **Runtime Marshalling Disabled:** We disable runtime marshalling via `[assembly: DisableRuntimeMarshalling]`, ensuring C# structures are passed direct bit-for-bit (blitted) without conversions.
3. **No Garbage Collection Transitions:** High-frequency APIs are marked with `[SuppressGCTransition]` to prevent GC overhead on every layout query.