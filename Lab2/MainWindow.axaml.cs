using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Lab2;

public partial class MainWindow : Window
{
    private string _selectedShape = "Point";

    public MainWindow()
    {
        InitializeComponent();
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
    }

    private void Help_Click(object sender, RoutedEventArgs e)
    {

    }
}