using Avalonia;
using Avalonia.Media;
using Lab2.Shapes;

namespace Lab2.Editors;

public sealed class LineEditor : EditorBase
{
    private bool _drawing;
    private Point _start;
    private Point _current;

    public override void OnMouseDown(Point p)
    {
        _drawing = true;
        _start = _current = p;
    }

    public override void OnMouseMove(Point p)
    {
        if (!_drawing) return;
        _current = p;
        Canvas.InvalidateVisual();
    }

    public override void OnMouseUp(Point p)
    {
        if (!_drawing) return;
        _drawing = false;
        Canvas.AddShape(new LineShape(_start, p));
        Canvas.InvalidateVisual();
    }

    public override void PaintPreview(DrawingContext ctx)
    {
        if (!_drawing) return;
        var pen = new Pen(Brushes.Black, 1);
        ctx.DrawLine(pen, _start, _current);
    }
}
