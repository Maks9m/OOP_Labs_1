using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Lab3;

public partial class MainWindow : Window
{
    private string _selectedShape = "Point";

    public MainWindow()
    {
        InitializeComponent();
        
        // Застосовуємо параметри варіанту 17 до canvas
        EditorCanvas.InitializeForVariant17();
        
        UpdateWindowTitle();
    }

    private void File_Click(object sender, RoutedEventArgs e)
    {

    }

    private void Object_Click(object sender, RoutedEventArgs e)
    {

    }

    private void ShapeRadio_Checked(object? sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton rb) return;
        _selectedShape = rb.Tag as string ?? _selectedShape;

        // Оновлюємо режим canvas
        switch (_selectedShape)
        {
            case "Point":
                EditorCanvas.Mode = Controls.ShapeKind.Point;
                break;
            case "Line":
                EditorCanvas.Mode = Controls.ShapeKind.Line;
                break;
            case "Rectangle":
                EditorCanvas.Mode = Controls.ShapeKind.Rectangle;
                break;
            case "Ellipse":
                EditorCanvas.Mode = Controls.ShapeKind.Ellipse;
                break;
            default:
                EditorCanvas.Mode = Controls.ShapeKind.Point;
                break;
        }
        
        // Синхронізуємо toolbar кнопки
        SynchronizeToolbarButtons();
        UpdateWindowTitle();
    }
    
    private void Toolbar_Toggle_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton btn) return;
        
        // Якщо кнопка була активна і її натиснули знову, не дозволяємо скинути
        if (btn.IsChecked == false)
        {
            btn.IsChecked = true;
            return;
        }
        
        // Визначаємо новий режим
        string newShape;
        if (sender == TbPoint)
        {
            newShape = "Point";
            EditorCanvas.Mode = Controls.ShapeKind.Point;
        }
        else if (sender == TbLine)
        {
            newShape = "Line";
            EditorCanvas.Mode = Controls.ShapeKind.Line;
        }
        else if (sender == TbEllipse)
        {
            newShape = "Ellipse";
            EditorCanvas.Mode = Controls.ShapeKind.Ellipse;
        }
        else if (sender == TbRect)
        {
            newShape = "Rectangle";
            EditorCanvas.Mode = Controls.ShapeKind.Rectangle;
        }
        else
        {
            return;
        }
        
        _selectedShape = newShape;
        
        // Синхронізуємо всі елементи
        SynchronizeMenuItems();
        SynchronizeToolbarButtons();
        UpdateWindowTitle();
    }

    private void SynchronizeToolbarButtons()
    {
        // Скидаємо всі кнопки
        TbPoint.IsChecked = false;
        TbLine.IsChecked = false;
        TbEllipse.IsChecked = false;
        TbRect.IsChecked = false;
        
        // Встановлюємо активну кнопку
        switch (_selectedShape)
        {
            case "Point":
                TbPoint.IsChecked = true;
                break;
            case "Line":
                TbLine.IsChecked = true;
                break;
            case "Rectangle":
                TbRect.IsChecked = true;
                break;
            case "Ellipse":
                TbEllipse.IsChecked = true;
                break;
        }
    }

    private void SynchronizeMenuItems()
    {
        // Скидаємо всі меню
        MenuPoint.IsChecked = false;
        MenuLine.IsChecked = false;
        MenuRect.IsChecked = false;
        MenuEllipse.IsChecked = false;
        
        // Встановлюємо активне меню
        switch (_selectedShape)
        {
            case "Point":
                MenuPoint.IsChecked = true;
                break;
            case "Line":
                MenuLine.IsChecked = true;
                break;
            case "Rectangle":
                MenuRect.IsChecked = true;
                break;
            case "Ellipse":
                MenuEllipse.IsChecked = true;
                break;
        }
    }

    private void Help_Click(object sender, RoutedEventArgs e)
    {

    }
    
    private void UpdateWindowTitle()
    {
        // Варіант 17: Ж mod 2 = 1, тому показуємо поточний тип в заголовку
        var shapeName = _selectedShape switch
        {
            "Point" => "Точка",
            "Line" => "Лінія",
            "Rectangle" => "Прямокутник",
            "Ellipse" => "Еліпс",
            _ => "Невідомо"
        };
        Title = $"Lab3 - Варіант 17 - Поточний об'єкт: {shapeName}";
    }
}