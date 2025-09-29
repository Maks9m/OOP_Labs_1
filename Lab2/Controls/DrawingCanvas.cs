using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Lab2.Shapes;
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

    private bool _isDrawing;
    private Point _start;
    private Point _current;

    public DrawingCanvas()
    {
        IsHitTestVisible = true;
    }

    public ShapeKind Mode
    {
        get => _mode;
        set { _mode = value; InvalidateVisual(); }
    }

    public IReadOnlyList<ShapeBase?> Shapes => _shapes;

    protected override Size MeasureOverride(Size availableSize)
    {
        // Take all available space so the surface is hit-testable
        return availableSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        // Occupy full assigned size
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
        _isDrawing = true;
        _start = _current = p;

        if (_mode == ShapeKind.Point)
        {
            AddShape(new PointShape(p));
            InvalidateVisual();
            _isDrawing = false;
        }
        e.Pointer.Capture(this);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (!_isDrawing) return;
        _current = e.GetPosition(this);
        InvalidateVisual(); // rubber band
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (!_isDrawing) return;
        _isDrawing = false;
        var end = e.GetPosition(this);

        switch (_mode)
        {
            case ShapeKind.Line:
                AddShape(new LineShape(_start, end));
                break;
            case ShapeKind.Rectangle:
                // Variant 16 (J=16): For rect (J mod 2 = 0) -> by opposite corners; Fill color (J mod 6 = 4) -> Orange
                AddShape(new RectShape(_start, end) { Fill = Brushes.Orange });
                break;
            case ShapeKind.Ellipse:
                // Variant 16 (J mod 2 = 0) for ellipse -> center-to-corner of bounding rect; Fill per (J mod 6 = 4) -> Orange? But for ellipse rules differ: (J mod 5 = 3 or 4) colored fill, (J mod 6 = 4) -> Orange.
                var cx = _start.X; var cy = _start.Y;
                var rect = new Rect(new Point(cx - (end.X - cx), cy - (end.Y - cy)), end).Normalize();
                AddShape(new EllipseShape(rect.TopLeft, rect.BottomRight) { Fill = Brushes.Orange });
                break;
        }
        e.Pointer.Capture(null);
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        // Subtle background so the drawing area is visible and clearly hit-testable
        var bgRect = new Rect(Bounds.Size);
        context.FillRectangle(Brushes.WhiteSmoke, bgRect);

        // Draw committed shapes
        foreach (var s in _shapes.Where(s => s is not null))
            s!.Render(context);

        // Rubber band preview (Variant 16: solid black line)
        if (_isDrawing && _mode != ShapeKind.Point)
        {
            var pen = new Pen(Brushes.Black, 1);
            if (_mode == ShapeKind.Line)
            {
                context.DrawLine(pen, _start, _current);
            }
            else if (_mode == ShapeKind.Rectangle)
            {
                var r = new Rect(_start, _current).Normalize();
                context.DrawRectangle(null, pen, r);
            }
            else if (_mode == ShapeKind.Ellipse)
            {
                var cx = _start.X; var cy = _start.Y;
                var r = new Rect(new Point(cx - (_current.X - cx), cy - (_current.Y - cy)), _current).Normalize();
                context.DrawEllipse(null, pen, r.Center, r.Width/2, r.Height/2);
            }
        }
    }

    private void AddShape(ShapeBase shape)
    {
        if (_shapes.Count >= Capacity)
            _shapes.RemoveAt(0); // drop oldest to maintain capacity
        _shapes.Add(shape);
    }
}
