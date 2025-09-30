using Avalonia;
using Avalonia.Media;
using Lab2.Shapes;

namespace Lab2.Editors;

public sealed class EllipseEditor : EditorBase
{
    private bool _drawing;
    private Point _center;
    private Point _current;

    public override void OnMouseDown(Point p)
    {
        _drawing = true;
        _center = _current = p;
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
        var rect = new Rect(new Point(_center.X - (p.X - _center.X), _center.Y - (p.Y - _center.Y)), p).Normalize();
        Canvas.AddShape(new EllipseShape(rect.TopLeft, rect.BottomRight) { Fill = Brushes.Orange });
        Canvas.InvalidateVisual();
    }

    public override void PaintPreview(DrawingContext ctx)
    {
        if (!_drawing) return;
        var pen = new Pen(Brushes.Black, 1);
        var r = new Rect(new Point(_center.X - (_current.X - _center.X), _center.Y - (_current.Y - _center.Y)), _current).Normalize();
        ctx.DrawEllipse(null, pen, r.Center, r.Width/2, r.Height/2);
    }
}
