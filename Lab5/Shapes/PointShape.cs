using Avalonia;
using Avalonia.Media;

namespace Lab5.Shapes;

public sealed class PointShape : ShapeBase
{
    public PointShape(Point p) : base(p, p) { }
    public PointShape(double x, double y) : base(new Point(x, y), new Point(x, y)) { }

    public override void Render(DrawingContext ctx)
    {
        // Draw a small filled circle to represent a point
        var radius = 2;
        var rect = new Rect(P1.X - radius, P1.Y - radius, radius * 2, radius * 2);
        ctx.DrawEllipse(Brushes.Black, null, rect.Center, radius, radius);
    }
    
    public override string GetShapeName() => "Точка";

    public override ShapeBase CreateInstance(Point startPoint)
    {
        return new PointShape(startPoint.X, startPoint.Y);
    }

    public override void PaintRubberBand(DrawingContext ctx, Pen pen, Point startPoint, Point currentPoint)
    {
        ctx.DrawEllipse(null, pen, currentPoint, 2, 2);
    }
}
