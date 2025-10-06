using Avalonia;
using Avalonia.Media;
using Lab3.Shapes;

namespace Lab3.Editors;

public sealed class PointEditor : EditorBase
{
    public override void OnMouseDown(Point p)
    {
        Canvas.AddShape(new PointShape(p));
        Canvas.InvalidateVisual();
    }

    public override void OnMouseMove(Point p) { }
    public override void OnMouseUp(Point p) { }

    public override void PaintPreview(DrawingContext ctx)
    {
        // Точка не потребує rubber band - вона створюється одразу
    }
}
