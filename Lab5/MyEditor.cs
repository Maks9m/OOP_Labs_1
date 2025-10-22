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
        foreach (var shape in _shapes.Where(s => s is not null))
        {
            shape!.Render(context);
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