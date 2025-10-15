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

    // Віртуальні методи для зменшення кількості switch cases
    public abstract ShapeBase CreateInstance(Point startPoint);
    
    public virtual void PaintRubberBand(DrawingContext ctx, Pen pen, Point startPoint, Point currentPoint)
    {
        // За замовчуванням - просто малюємо від початкової до поточної точки
        // Переозначається в дочірніх класах для специфічної логіки
    }

    // Віртуальний метод для обчислення координат при завершенні малювання
    public virtual void CalculateFinalCoordinates(Point startPoint, Point endPoint)
    {
        // За замовчуванням - прямо встановлюємо координати
        Set((long)startPoint.X, (long)startPoint.Y, (long)endPoint.X, (long)endPoint.Y);
    }
}
