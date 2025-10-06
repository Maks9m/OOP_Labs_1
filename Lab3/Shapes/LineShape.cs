using Avalonia;
using Avalonia.Media;

namespace Lab3.Shapes;

public sealed class LineShape : ShapeBase
{
    public Pen Pen { get; init; } = new Pen(Brushes.Black, 2);

    public LineShape(Point p1, Point p2) : base(p1, p2) { }

    public override void Render(DrawingContext ctx)
    {
        ctx.DrawLine(Pen, P1, P2);
    }
}
