using Avalonia;
using Avalonia.Media;

namespace Lab2.Shapes;

public sealed class PointShape : ShapeBase
{
    public PointShape(Point p) : base(p, p) { }

    public override void Render(DrawingContext ctx)
    {
        // Draw a small filled circle to represent a point
        var radius = 2;
        var rect = new Rect(P1.X - radius, P1.Y - radius, radius * 2, radius * 2);
        ctx.DrawEllipse(Brushes.Black, null, rect.Center, radius, radius);
    }
}
