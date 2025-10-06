using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Lab4.Shapes;
using Lab4.Controls;

namespace Lab4;

public partial class MainWindow : Window
{
    private string _selectedShape = "Point";

    public MainWindow()
    {
        InitializeComponent();
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

        // Встановлюємо новий тип фігури для редактора
        switch (_selectedShape)
        {
            case "Point":
                EditorCanvas.GetEditor().Start(new PointShape(0, 0));
                break;
            case "Line":
                EditorCanvas.GetEditor().Start(new LineShape(0, 0, 0, 0));
                break;
            case "Rectangle":
                EditorCanvas.GetEditor().Start(new RectShape(new Avalonia.Point(0, 0), new Avalonia.Point(0, 0)) { Fill = Brushes.Gray });
                break;
            case "Ellipse":
                EditorCanvas.GetEditor().Start(new EllipseShape(new Avalonia.Point(0, 0), new Avalonia.Point(0, 0)) { Fill = null });
                break;
            case "LineOO":
                EditorCanvas.GetEditor().Start(new LineOOShape(0, 0, 0, 0));
                break;
            case "Cube":
                EditorCanvas.GetEditor().Start(new CubeShape(new Avalonia.Point(0, 0), new Avalonia.Point(0, 0)));
                break;
            default:
                EditorCanvas.GetEditor().Start(new PointShape(0, 0));
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
            EditorCanvas.GetEditor().Start(new PointShape(0, 0));
        }
        else if (sender == TbLine)
        {
            newShape = "Line";
            EditorCanvas.GetEditor().Start(new LineShape(0, 0, 0, 0));
        }
        else if (sender == TbEllipse)
        {
            newShape = "Ellipse";
            EditorCanvas.GetEditor().Start(new EllipseShape(new Avalonia.Point(0, 0), new Avalonia.Point(0, 0)) { Fill = null });
        }
        else if (sender == TbRect)
        {
            newShape = "Rectangle";
            EditorCanvas.GetEditor().Start(new RectShape(new Avalonia.Point(0, 0), new Avalonia.Point(0, 0)) { Fill = Brushes.Gray });
        }
        else if (sender == TbLineOO)
        {
            newShape = "LineOO";
            EditorCanvas.GetEditor().Start(new LineOOShape(0, 0, 0, 0));
        }
        else if (sender == TbCube)
        {
            newShape = "Cube";
            EditorCanvas.GetEditor().Start(new CubeShape(new Avalonia.Point(0, 0), new Avalonia.Point(0, 0)));
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
        TbLineOO.IsChecked = false;
        TbCube.IsChecked = false;
        
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
            case "LineOO":
                TbLineOO.IsChecked = true;
                break;
            case "Cube":
                TbCube.IsChecked = true;
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
        MenuLineOO.IsChecked = false;
        MenuCube.IsChecked = false;
        
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
            case "LineOO":
                MenuLineOO.IsChecked = true;
                break;
            case "Cube":
                MenuCube.IsChecked = true;
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
            "LineOO" => "Лінія з кружечками",
            "Cube" => "Каркас куба",
            _ => "Невідомо"
        };
        Title = $"Lab4 - Варіант 17 - Поточний об'єкт: {shapeName}";
    }
}