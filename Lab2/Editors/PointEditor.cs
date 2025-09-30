using Avalonia;
using Avalonia.Media;
using Lab2.Shapes;

namespace Lab2.Editors;

public sealed class PointEditor : EditorBase
{
    public override void OnMouseDown(Point p)
    {
        Canvas.AddShape(new PointShape(p));
        Canvas.InvalidateVisual();
    }

    public override void OnMouseMove(Point p) { }
    public override void OnMouseUp(Point p) { }

    public override void PaintPreview(DrawingContext ctx) { }
}
