using Avalonia;
using Avalonia.Media;

namespace Lab4.Shapes;

// Лінія з кружечками - множинне успадкування через інтерфейси
public sealed class LineOOShape : ShapeBase
{
    private LineShape _lineShape;
    private EllipseShape _ellipse1;
    private EllipseShape _ellipse2;

    public LineOOShape(double x1, double y1, double x2, double y2) : base(new Point(x1, y1), new Point(x2, y2))
    {
        _lineShape = new LineShape(x1, y1, x2, y2);
        
        // Кружечки на кінцях лінії
        const double radius = 5;
        _ellipse1 = new EllipseShape(
            new Point(x1 - radius, y1 - radius),
            new Point(x1 + radius, y1 + radius)
        ) { Fill = Brushes.Blue };
        
        _ellipse2 = new EllipseShape(
            new Point(x2 - radius, y2 - radius),
            new Point(x2 + radius, y2 + radius)
        ) { Fill = Brushes.Blue };
    }

    public override void Set(long x1, long y1, long x2, long y2)
    {
        _lineShape.Set(x1, y1, x2, y2);
        
        const double radius = 5;
        _ellipse1.Set((long)(x1 - radius), (long)(y1 - radius), 
                     (long)(x1 + radius), (long)(y1 + radius));
        _ellipse2.Set((long)(x2 - radius), (long)(y2 - radius), 
                     (long)(x2 + radius), (long)(y2 + radius));
    }

    public override void Render(DrawingContext context)
    {
        // Використовуємо методи Show базових класів (поліморфізм)
        _lineShape.Render(context);
        _ellipse1.Render(context);
        _ellipse2.Render(context);
    }
}