using Avalonia;
using Avalonia.Media;

namespace Lab4.Shapes;

public interface ILineDrawable
{
  void DrawLine(DrawingContext ctx);
}