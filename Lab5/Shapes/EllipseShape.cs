using Avalonia;
using Avalonia.Media;

namespace Lab5.Shapes;

public sealed class EllipseShape : ShapeBase, IEllipseDrawable
{
    public IBrush? Fill { get; init; }
    public Pen Pen { get; init; } = new Pen(Brushes.Black, 1);

    public EllipseShape(Point p1, Point p2) : base(p1, p2) { }
    public EllipseShape(int x1, int y1, int x2, int y2) : base(new Point(x1, y1), new Point(x2, y2)) { }

    public override void Render(DrawingContext ctx)
    {
        var rect = new Rect(P1, P2).Normalize();
        ctx.DrawEllipse(Fill, Pen, rect.Center, rect.Width/2, rect.Height/2);
    }
    
    public override string GetShapeName() => "Еліпс";

    public override ShapeBase CreateInstance(Point startPoint)
    {
        return new EllipseShape(startPoint, startPoint) { Fill = null };
    }

    public override void PaintRubberBand(DrawingContext ctx, Pen pen, Point startPoint, Point currentPoint)
    {
        // Варіант 17: еліпс по двом протилежним кутам
        var ellipseRect = new Rect(startPoint, currentPoint).Normalize();
        ctx.DrawEllipse(null, pen, ellipseRect.Center, ellipseRect.Width/2, ellipseRect.Height/2);
    }

    public void DrawEllipse(DrawingContext ctx)
    {
        Render(ctx);
    }
}
