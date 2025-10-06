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
        
        // Створюємо копію поточної фігури для редагування
        _currentShape = _currentShape switch
        {
            PointShape => new PointShape(point.X, point.Y),
            LineShape => new LineShape(point.X, point.Y, point.X, point.Y),
            RectShape => new RectShape(point, point) { Fill = Brushes.Gray },
            EllipseShape => new EllipseShape(point, point) { Fill = null },
            LineOOShape => new LineOOShape(point.X, point.Y, point.X, point.Y),
            CubeShape => new CubeShape(point, point),
            _ => null
        };
    }

    public void OnLBup(Point point)
    {
        if (!_isDrawing || _currentShape == null) return;
        
        _isDrawing = false;
        
        // Оновлюємо координати фігури відповідно до варіанту 17
        switch (_currentShape)
        {
            case RectShape:
            case CubeShape:
                // Варіант 17: прямокутник і куб від центру до кута
                var rect = BuildRectFromCenter(_startPoint, point);
                _currentShape.Set(
                    (long)rect.TopLeft.X, (long)rect.TopLeft.Y,
                    (long)rect.BottomRight.X, (long)rect.BottomRight.Y
                );
                break;
            case EllipseShape:
                // Варіант 17: еліпс по двом протилежним кутам
                _currentShape.Set(
                    (long)_startPoint.X, (long)_startPoint.Y, 
                    (long)point.X, (long)point.Y
                );
                break;
            default:
                // Точки, лінії, LineOO - прямо як є
                _currentShape.Set(
                    (long)_startPoint.X, (long)_startPoint.Y, 
                    (long)point.X, (long)point.Y
                );
                break;
        }
        
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

        switch (_currentShape)
        {
            case PointShape:
                context.DrawEllipse(null, pen, _currentPoint, 2, 2);
                break;
            case LineShape:
                context.DrawLine(pen, _startPoint, _currentPoint);
                break;
            case RectShape:
                // Варіант 17: прямокутник від центру до кута
                var rectFromCenter = BuildRectFromCenter(_startPoint, _currentPoint);
                context.DrawRectangle(null, pen, rectFromCenter);
                break;
            case EllipseShape:
                // Варіант 17: еліпс по двом протилежним кутам
                var ellipseRect = new Rect(_startPoint, _currentPoint).Normalize();
                context.DrawEllipse(null, pen, ellipseRect.Center, 
                    ellipseRect.Width/2, ellipseRect.Height/2);
                break;
            case LineOOShape:
                // Лінія з кружечками
                context.DrawLine(pen, _startPoint, _currentPoint);
                context.DrawEllipse(null, pen, _startPoint, 5, 5);
                context.DrawEllipse(null, pen, _currentPoint, 5, 5);
                break;
            case CubeShape:
                DrawCubeRubberBand(context, pen);
                break;
        }
    }

    private void DrawCubeRubberBand(DrawingContext context, Pen pen)
    {
        // Для куба в rubber band використовуємо ту ж логіку що і для кінцевої фігури
        // Спочатку будуємо прямокутник від центру до кута
        var rect = BuildRectFromCenter(_startPoint, _currentPoint);
        var depth = Math.Min(rect.Width, rect.Height) / 3;
        
        // Передній прямокутник
        context.DrawRectangle(null, pen, rect);
        
        // Задній прямокутник (зміщений)
        var backRect = new Rect(
            rect.X + depth, rect.Y - depth,
            rect.Width, rect.Height
        );
        context.DrawRectangle(null, pen, backRect);
        
        // З'єднувальні лінії
        context.DrawLine(pen, rect.TopLeft, backRect.TopLeft);
        context.DrawLine(pen, rect.TopRight, backRect.TopRight);
        context.DrawLine(pen, rect.BottomLeft, backRect.BottomLeft);
        context.DrawLine(pen, rect.BottomRight, backRect.BottomRight);
    }

    private Rect BuildRectFromCenter(Point center, Point corner)
    {
        // Варіант 17: прямокутник від центру до кута
        var centerX = center.X;
        var centerY = center.Y;
        var cornerX = corner.X;
        var cornerY = corner.Y;
        
        var x1 = 2 * centerX - cornerX;
        var y1 = 2 * centerY - cornerY;
        
        return new Rect(
            new Point(x1, y1),
            new Point(cornerX, cornerY)
        ).Normalize();
    }

    private Rect BuildRect(Point start, Point end)
    {
        // Варіант 17: прямокутник від центру до кута
        var centerX = start.X;
        var centerY = start.Y;
        var cornerX = end.X;
        var cornerY = end.Y;
        
        var x1 = 2 * centerX - cornerX;
        var y1 = 2 * centerY - cornerY;
        
        return new Rect(
            new Point(x1, y1),
            new Point(cornerX, cornerY)
        ).Normalize();
    }

    private void AddShape(ShapeBase shape)
    {
        if (_shapes.Count >= CAPACITY)
            _shapes.RemoveAt(0);
        _shapes.Add(shape);
    }

    public IReadOnlyList<ShapeBase?> Shapes => _shapes;
}