using Avalonia;
using Avalonia.Media;
using Lab2.Controls;
using Lab2.Shapes;

namespace Lab2.Editors;

public abstract class EditorBase
{
    protected DrawingCanvas Canvas { get; private set; } = default!;

    public void SetCanvas(DrawingCanvas canvas) => Canvas = canvas;

    public abstract void OnMouseDown(Point p);
    public abstract void OnMouseMove(Point p);
    public abstract void OnMouseUp(Point p);
    public abstract void PaintPreview(DrawingContext ctx);
}
