using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Lab4.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab4;

public sealed class MyEditor
{
    // Варіант 17: статичний масив Shape[117]
    private const int CAPACITY = 117;
    private readonly List<ShapeBase?> _shapes = new();
    private ShapeBase? _currentShape;
    private bool _isDrawing;
    private Point _startPoint;
    private Point _currentPoint;

    public MyEditor()
    {
        // Конструктор
    }

    ~MyEditor()
    {
        // Деструктор - в C# не потрібен через GC
    }

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
    }

    public IReadOnlyList<ShapeBase?> Shapes => _shapes;
}