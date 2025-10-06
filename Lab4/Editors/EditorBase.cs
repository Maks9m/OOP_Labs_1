using Avalonia;
using Avalonia.Media;
using Lab4.Controls;
using Lab4.Shapes;

namespace Lab4.Editors;

public abstract class EditorBase
{
    protected DrawingCanvas Canvas { get; private set; } = default!;

    public void SetCanvas(DrawingCanvas canvas) => Canvas = canvas;

    public abstract void OnMouseDown(Point p);
    public abstract void OnMouseMove(Point p);
    public abstract void OnMouseUp(Point p);
    public abstract void PaintPreview(DrawingContext ctx);
}
