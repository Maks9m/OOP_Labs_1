using Avalonia;
using Avalonia.Media;

namespace Lab3.Shapes;

public sealed class RectShape : ShapeBase
{
    public IBrush? Fill { get; init; }
    public Pen Pen { get; init; } = new Pen(Brushes.Black, 1);

    public RectShape(Point p1, Point p2) : base(p1, p2) { }

    public override void Render(DrawingContext ctx)
    {
        var rect = new Rect(P1, P2).Normalize();
        ctx.DrawRectangle(Fill, Pen, rect);
    }
}
