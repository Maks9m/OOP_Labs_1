using Avalonia;
using Avalonia.Media;
using Lab3.Shapes;

namespace Lab3.Editors;

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
        // Варіант 17: Ж mod 4 = 1 -> суцільна лінія червоного кольору
        var pen = new Pen(Brushes.Red, 1);
        ctx.DrawLine(pen, _start, _current);
    }
}
