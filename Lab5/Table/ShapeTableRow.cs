using System;

namespace Lab5.Table;

// Клас для представлення рядка таблиці
public class ShapeTableRow
{
    public string Name { get; set; } = string.Empty;
    public int X1 { get; set; }
    public int Y1 { get; set; }
    public int X2 { get; set; }
    public int Y2 { get; set; }
    
    public ShapeTableRow(string name, int x1, int y1, int x2, int y2)
    {
        Name = name;
        X1 = x1;
        Y1 = y1;
        X2 = x2;
        Y2 = y2;
    }
}
