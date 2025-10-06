using Avalonia;
using Avalonia.Media;
using Lab4.Shapes;

namespace Lab4.Editors;

public sealed class EllipseEditor : EditorBase
{
    private bool _drawing;
    private Point _anchor;
    private Point _current;

    public override void OnMouseDown(Point p)
    {
        _drawing = true;
        _anchor = _current = p;
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
        var (p1, p2) = BuildRectPoints(_anchor, p);
        var rect = new Rect(p1, p2).Normalize();
        
        // Варіант 17: Ж mod 5 = 2 -> чорний контур еліпсу без заповнення
        Canvas.AddShape(new EllipseShape(rect.TopLeft, rect.BottomRight) { Fill = null });
        Canvas.InvalidateVisual();
    }

    public override void PaintPreview(DrawingContext ctx)
    {
        if (!_drawing) return;
        
        // Варіант 17: Ж mod 4 = 1 -> суцільна лінія червоного кольору
        var pen = new Pen(Brushes.Red, 1);
        var (p1, p2) = BuildRectPoints(_anchor, _current);
        var r = new Rect(p1, p2).Normalize();
        ctx.DrawEllipse(null, pen, r.Center, r.Width/2, r.Height/2);
    }

    private static (Point p1, Point p2) BuildRectPoints(Point a, Point b)
    {
        // Варіант 17: Ж mod 2 = 1 -> еліпс по двом протилежним кутам
        return (a, b);
    }
}
