using Avalonia.Media;

namespace Lab5.Shapes;

/// <summary>
/// Інтерфейс для об'єктів, які можуть малювати прямокутники
/// Частина множинного успадкування інтерфейсів
/// </summary>
public interface IRectDrawable
{
    /// <summary>
    /// Малює прямокутник використовуючи метод Render з класу RectShape
    /// </summary>
    void DrawRect(DrawingContext ctx);
}
