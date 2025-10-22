using Avalonia.Media;

namespace Lab5.Shapes;

/// <summary>
/// Інтерфейс для об'єктів, які можуть малювати лінії
/// Частина множинного успадкування інтерфейсів
/// </summary>
public interface ILineDrawable
{
    /// <summary>
    /// Малює лінію використовуючи метод Render з класу LineShape
    /// </summary>
    void DrawLine(DrawingContext ctx);
}
