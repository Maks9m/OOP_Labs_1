using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Lab5.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lab5;

public sealed class MyEditor
{
    private static MyEditor? _instance;
    private static readonly object _lock = new();
    
    private MyEditor()
    {
        // Конструктор
    }
    
    // Статичний метод для доступу до єдиного екземпляру
    public static MyEditor Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MyEditor();
                    }
                }
            }
            return _instance;
        }
    }
    
    // Варіант 16: статичний масив Shape[116]
    private const int CAPACITY = 116;
    private readonly List<ShapeBase?> _shapes = new();
    private ShapeBase? _currentShape;
    private bool _isDrawing;
    private Point _startPoint;
    private Point _currentPoint;

    // Event для оповіщення про додавання нової фігури
    public event Action<ShapeBase>? ShapeAdded;
    // Event для оповіщення про видалення фігури
    public event Action<int>? ShapeRemoved;
    
    // Шлях до файлу для збереження фігур
    private const string ShapesFilePath = "shapes.txt";

    public void Start(ShapeBase shape)
    {
        _currentShape = shape;
        _isDrawing = false;
    }

    public void OnLBdown(Point point)
    {
        if (_currentShape == null) return;
        
        _isDrawing = true;
        _startPoint = _currentPoint = point;
        
        // Використовуємо поліморфізм замість switch
        _currentShape = _currentShape.CreateInstance(point);
    }

    public void OnLBup(Point point)
    {
        if (!_isDrawing || _currentShape == null) return;
        
        _isDrawing = false;
        
        // Використовуємо поліморфізм замість switch
        _currentShape.CalculateFinalCoordinates(_startPoint, point);
        
        // Додаємо фігуру до колекції
        AddShape(_currentShape);
    }

    public void OnMouseMove(Point point)
    {
        if (!_isDrawing) return;
        _currentPoint = point;
    }

    public void OnPaint(DrawingContext context, Size size)
    {
        // Малюємо фон
        var bgRect = new Rect(size);
        context.FillRectangle(Brushes.WhiteSmoke, bgRect);

        // Малюємо всі збережені фігури
        for (int i = 0; i < _shapes.Count; i++)
        {
            var shape = _shapes[i];
            if (shape is null) continue;
            shape.Render(context);

            // Якщо ця фігура підсвічена, малюємо навколо неї прямокутник-підсвітку
            if (_highlightIndex.HasValue && _highlightIndex.Value == i)
            {
                var x1 = Math.Min(shape.P1.X, shape.P2.X);
                var y1 = Math.Min(shape.P1.Y, shape.P2.Y);
                var x2 = Math.Max(shape.P1.X, shape.P2.X);
                var y2 = Math.Max(shape.P1.Y, shape.P2.Y);
                var w = x2 - x1;
                var h = y2 - y1;
                var highlightPen = new Pen(Brushes.Red, 2) { DashStyle = new DashStyle(new double[] {4,4}, 0) };

                // Special-case small/point shapes: draw an ellipse marker so the highlight is visible
                if (w < 4 && h < 4)
                {
                    // Draw a small circle around the point
                    var center = shape.P1;
                    double r = 8.0;
                    // DrawEllipse(brush, pen, center, radiusX, radiusY)
                    context.DrawEllipse(null, highlightPen, center, r, r);
                }
                else
                {
                    var rect = new Rect(new Point(x1, y1), new Point(x2, y2));
                    context.DrawRectangle(null, highlightPen, rect);
                }
            }
        }

        // Малюємо rubber band
        if (_isDrawing && _currentShape != null)
        {
            PaintRubberBand(context);
        }
    }

    private void PaintRubberBand(DrawingContext context)
    {
        if (_currentShape == null) return;

        // Варіант 17: пунктирна лінія для rubber band
        var pen = new Pen(Brushes.Black, 1) 
        { 
            DashStyle = new DashStyle(new double[] { 2, 2 }, 0) 
        };

        // Використовуємо поліморфізм замість switch
        _currentShape.PaintRubberBand(context, pen, _startPoint, _currentPoint);
    }

    public void LoadShapesFromFile(string path)
    {
        if (!File.Exists(path)) return;

        foreach (var line in File.ReadAllLines(path))
        {
            var parts = line.Split('\t');
            if (parts.Length < 5) continue;
            string name = parts[0].Trim();
            if (!int.TryParse(parts[1], out int x1)) continue;
            int.TryParse(parts[2], out int y1);
            int.TryParse(parts[3], out int x2);
            int.TryParse(parts[4], out int y2);

            ShapeBase? shape = name switch
            {
                "Точка" => new PointShape(x1, y1),
                "Лінія" => new LineShape(x1, y1, x2, y2),
                "Прямокутник" => new RectShape(x1, y1, x2, y2),
                "Еліпс" => new EllipseShape(x1, y1, x2, y2),
                "Лінія з кружечками" => new LineOOShape(x1, y1, x2, y2),
                "Куб" => new CubeShape(x1, y1, x2, y2),
                _ => null
            };

            if (shape != null)
            {
                _shapes.Add(shape);
            }
        }
    }

    // Highlight a shape at given index (or clear if null)
    private int? _highlightIndex;
    public void HighlightShape(int? index)
    {
        if (index.HasValue && (index.Value < 0 || index.Value >= _shapes.Count))
            _highlightIndex = null;
        else
            _highlightIndex = index;
    }

    // Remove shape at given index and rewrite storage file
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _shapes.Count) return;
        _shapes.RemoveAt(index);
        // Update persistent storage by rewriting the file
        RewriteShapesFile();
        ShapeRemoved?.Invoke(index);
    }

    private void RewriteShapesFile()
    {
        try
        {
            using var writer = new StreamWriter(ShapesFilePath, append: false);
            foreach (var shape in _shapes.Where(s => s is not null))
            {
                var s = shape!;
                var line = $"{s.GetShapeName()}\t{(int)s.P1.X}\t{(int)s.P1.Y}\t{(int)s.P2.X}\t{(int)s.P2.Y}";
                writer.WriteLine(line);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка перезапису файлу: {ex.Message}");
        }
    }

    private void AddShape(ShapeBase shape)
    {
        if (_shapes.Count >= CAPACITY)
            _shapes.RemoveAt(0);
        _shapes.Add(shape);
        
        // Записуємо фігуру у файл
        SaveShapeToFile(shape);
        
        // Викликаємо подію про додавання фігури
        ShapeAdded?.Invoke(shape);
    }
    
    // Запис фігури у текстовий файл
    private void SaveShapeToFile(ShapeBase shape)
    {
        try
        {
            using var writer = new StreamWriter(ShapesFilePath, append: true);
            var line = $"{shape.GetShapeName()}\t{(int)shape.P1.X}\t{(int)shape.P1.Y}\t{(int)shape.P2.X}\t{(int)shape.P2.Y}";
            writer.WriteLine(line);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка запису у файл: {ex.Message}");
        }
    }

    public IReadOnlyList<ShapeBase?> Shapes => _shapes;
}