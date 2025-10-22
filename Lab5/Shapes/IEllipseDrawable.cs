using Avalonia.Media;

namespace Lab5.Shapes;

/// <summary>
/// Інтерфейс для об'єктів, які можуть малювати еліпси/кружечки
/// Частина множинного успадкування інтерфейсів
/// </summary>
public interface IEllipseDrawable
{
    /// <summary>
    /// Малює еліпс використовуючи метод Render з класу EllipseShape
    /// </summary>
    void DrawEllipse(DrawingContext ctx);
}
