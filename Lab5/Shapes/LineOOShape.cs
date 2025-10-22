using Avalonia;
using Avalonia.Media;

namespace Lab5.Shapes;

// Лінія з кружечками - множинне успадкування через інтерфейси
public sealed class LineOOShape : ShapeBase, ILineDrawable, IEllipseDrawable
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
        )
        { Fill = Brushes.Blue };

        _ellipse2 = new EllipseShape(
            new Point(x2 - radius, y2 - radius),
            new Point(x2 + radius, y2 + radius)
        )
        { Fill = Brushes.Blue };
    }

    public override void Set(long x1, long y1, long x2, long y2)
    {
        // Оновлюємо базові координати
        base.Set(x1, y1, x2, y2);

        _lineShape.Set(x1, y1, x2, y2);

        const double radius = 5;
        _ellipse1.Set((long)(x1 - radius), (long)(y1 - radius),
                     (long)(x1 + radius), (long)(y1 + radius));
        _ellipse2.Set((long)(x2 - radius), (long)(y2 - radius),
                     (long)(x2 + radius), (long)(y2 + radius));
    }

    public override void Render(DrawingContext context)
    {
        DrawLine(context);
        DrawEllipse(context);
    }

    public void DrawLine(DrawingContext ctx)
    {
        _lineShape.DrawLine(ctx);
    }

    public void DrawEllipse(DrawingContext ctx)
    {
        _ellipse1.DrawEllipse(ctx);
        _ellipse2.DrawEllipse(ctx);
    }

    public override string GetShapeName() => "Лінія з кружечками";

    public override ShapeBase CreateInstance(Point startPoint)
    {
        return new LineOOShape(startPoint.X, startPoint.Y, startPoint.X, startPoint.Y);
    }

    public override void PaintRubberBand(DrawingContext ctx, Pen pen, Point startPoint, Point currentPoint)
    {
        const double radius = 5;
        ctx.DrawLine(pen, startPoint, currentPoint);
        ctx.DrawEllipse(null, pen, startPoint, radius, radius);
        ctx.DrawEllipse(null, pen, currentPoint, radius, radius);
    }
}