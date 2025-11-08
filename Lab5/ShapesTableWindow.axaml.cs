using System;
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
        // Коли вікно відкриється, надаємо фокус таблиці щоб клавіша Delete працювала
        this.Opened += (s, e) =>
        {
            // Якщо TableGrid існує, намагаємось задати фокус
            try
            {
                TableGrid.Focus();
            }
            catch { }
        };
    }

    // Raised when the user selects a row in the table.
    public event EventHandler<int?>? RowSelected;

    // Raised when the user requests deletion (Delete key) of the selected row.
    public event EventHandler<int>? RowDeleteRequested;

        private void TableGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (TableGrid.SelectedIndex >= 0)
            {
                RowSelected?.Invoke(this, TableGrid.SelectedIndex);
            }
            else
            {
                RowSelected?.Invoke(this, null);
            }
        }

        private void TableGrid_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            // If Delete key pressed, request deletion of the selected row
            if (e.Key == Avalonia.Input.Key.Delete && TableGrid.SelectedIndex >= 0)
            {
                RowDeleteRequested?.Invoke(this, TableGrid.SelectedIndex);
                e.Handled = true;
            }
        }
}
