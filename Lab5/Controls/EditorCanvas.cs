using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Lab5.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace Lab5.Controls;

public sealed class EditorCanvas : Control
{
    // Варіант 16: використання класичного Singleton
    private MyEditor Editor => MyEditor.Instance;
    
    public EditorCanvas()
    {
        IsHitTestVisible = true;
        Editor.Start(new PointShape(0, 0)); // Ініціалізуємо з точкою
    }

    public MyEditor GetEditor() => Editor;

    protected override Size MeasureOverride(Size availableSize)
    {
        return availableSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return finalSize;
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        Cursor = new Cursor(StandardCursorType.Cross);
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        Cursor = new Cursor(StandardCursorType.Arrow);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var p = e.GetPosition(this);
        Focus();
        Editor.OnLBdown(p);
        e.Pointer.Capture(this);
        InvalidateVisual();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        var p = e.GetPosition(this);
        Editor.OnMouseMove(p);
        InvalidateVisual();
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        var p = e.GetPosition(this);
        Editor.OnLBup(p);
        e.Pointer.Capture(null);
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        Editor.OnPaint(context, Bounds.Size);
    }
}