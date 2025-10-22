using Avalonia;
using Avalonia.Media;

namespace Lab5.Shapes;

public sealed class LineShape : ShapeBase, ILineDrawable
{
    public Pen Pen { get; init; } = new Pen(Brushes.Black, 1);

    public LineShape(Point p1, Point p2) : base(p1, p2) { }
    public LineShape(double x1, double y1, double x2, double y2) : base(new Point(x1, y1), new Point(x2, y2)) { }

    public override void Render(DrawingContext ctx)
    {
        ctx.DrawLine(Pen, P1, P2);
    }
    
    public override string GetShapeName() => "Лінія";

    public override ShapeBase CreateInstance(Point startPoint)
    {
        return new LineShape(startPoint.X, startPoint.Y, startPoint.X, startPoint.Y);
    }

    public override void PaintRubberBand(DrawingContext ctx, Pen pen, Point startPoint, Point currentPoint)
    {
        ctx.DrawLine(pen, startPoint, currentPoint);
    }

    public void DrawLine(DrawingContext ctx)
    {
        Render(ctx);
    }
}
