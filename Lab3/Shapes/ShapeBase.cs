using Avalonia;
using Avalonia.Media;

namespace Lab3.Shapes;

public abstract class ShapeBase
{
    public Point P1 { get; set; }
    public Point P2 { get; set; }

    protected ShapeBase(Point p1, Point p2)
    {
        P1 = p1; P2 = p2;
    }

    public abstract void Render(DrawingContext ctx);
}
