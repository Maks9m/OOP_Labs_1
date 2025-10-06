using Avalonia;
using Avalonia.Media;

namespace Lab4.Shapes;

public abstract class ShapeBase
{
    public Point P1 { get; set; }
    public Point P2 { get; set; }

    protected ShapeBase(Point p1, Point p2)
    {
        P1 = p1; P2 = p2;
    }

    // Метод для встановлення координат (як у C++ версії)
    public virtual void Set(long x1, long y1, long x2, long y2)
    {
        P1 = new Point(x1, y1);
        P2 = new Point(x2, y2);
    }

    public abstract void Render(DrawingContext ctx);
}
