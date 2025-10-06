using Avalonia;
using Avalonia.Media;

namespace Lab4.Shapes;

public sealed class EllipseShape : ShapeBase
{
    public IBrush? Fill { get; init; }
    public Pen Pen { get; init; } = new Pen(Brushes.Black, 1);

    public EllipseShape(Point p1, Point p2) : base(p1, p2) { }

    public override void Render(DrawingContext ctx)
    {
        var rect = new Rect(P1, P2).Normalize();
        ctx.DrawEllipse(Fill, Pen, rect.Center, rect.Width/2, rect.Height/2);
    }
}
