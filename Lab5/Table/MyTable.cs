using System.Collections.ObjectModel;

namespace Lab5.Table;

// Незалежний модуль таблиці
// Вимога: не залежить від інших модулів проекту (окрім стандартних)
public class MyTable
{
    private readonly ObservableCollection<ShapeTableRow> _rows = new();
    
    public ObservableCollection<ShapeTableRow> Rows => _rows;
    
    // Додавання нового рядка до таблиці
    public void Add(string shapeName, int x1, int y1, int x2, int y2)
    {
        var row = new ShapeTableRow(shapeName, x1, y1, x2, y2);
        _rows.Add(row);
    }
    
    // Очищення таблиці
    public void Clear()
    {
        _rows.Clear();
    }
    
    // Отримання кількості рядків
    public int Count => _rows.Count;
}
