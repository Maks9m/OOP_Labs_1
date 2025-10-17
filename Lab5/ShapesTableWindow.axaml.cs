using Avalonia.Controls;
using Lab5.Table;

namespace Lab5;

public partial class ShapesTableWindow : Window
{
    public ShapesTableWindow() : this(new MyTable())
    {
        // Конструктор за замовчуванням для XAML previewer
    }
    
    public ShapesTableWindow(MyTable table)
    {
        InitializeComponent();
        
        // Встановлюємо DataContext для прив'язки
        DataContext = table;
    }
}
