using Avalonia;
using Avalonia.Media;
using Lab3.Controls;
using Lab3.Shapes;

namespace Lab3.Editors;

public abstract class EditorBase
{
    protected DrawingCanvas Canvas { get; private set; } = default!;

    public void SetCanvas(DrawingCanvas canvas) => Canvas = canvas;

    public abstract void OnMouseDown(Point p);
    public abstract void OnMouseMove(Point p);
    public abstract void OnMouseUp(Point p);
    public abstract void PaintPreview(DrawingContext ctx);
}
