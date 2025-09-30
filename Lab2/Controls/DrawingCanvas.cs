using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Lab2.Shapes;
using Lab2.Editors;
using System.Collections.Generic;
using System.Linq;

namespace Lab2.Controls;

public enum ShapeKind { Point, Line, Rectangle, Ellipse }

public sealed class DrawingCanvas : Control
{
    // Variant 16: N = J + 100 = 16 + 100 = 116
    private const int Capacity = 116;

    private readonly List<ShapeBase?> _shapes = new(Capacity);
    private ShapeKind _mode = ShapeKind.Point;
    private EditorBase _editor = new PointEditor();

    public DrawingCanvas()
    {
        IsHitTestVisible = true;
    }

    public ShapeKind Mode
    {
        get => _mode;
        set
        {
            _mode = value;
            _editor = _mode switch
            {
                ShapeKind.Point => new PointEditor(),
                ShapeKind.Line => new LineEditor(),
                ShapeKind.Rectangle => new RectEditor(),
                ShapeKind.Ellipse => new EllipseEditor(),
                _ => new PointEditor(),
            };
            _editor.SetCanvas(this);
            InvalidateVisual();
        }
    }

    public IReadOnlyList<ShapeBase?> Shapes => _shapes;

    public void AddShape(ShapeBase shape)
    {
        if (_shapes.Count >= Capacity)
            _shapes.RemoveAt(0);
        _shapes.Add(shape);
    }

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
        _editor.SetCanvas(this);
        _editor.OnMouseDown(p);
        e.Pointer.Capture(this);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        var p = e.GetPosition(this);
        _editor.OnMouseMove(p);
        InvalidateVisual();
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        var p = e.GetPosition(this);
        _editor.OnMouseUp(p);
        e.Pointer.Capture(null);
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bgRect = new Rect(Bounds.Size);
        context.FillRectangle(Brushes.WhiteSmoke, bgRect);

        foreach (var s in _shapes.Where(s => s is not null))
            s!.Render(context);

        _editor.SetCanvas(this);
        _editor.PaintPreview(context);
    }
}
