using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Object3.Controls;

public class ChartCanvas : Canvas
{
    private List<(int x, int y)> _dataPoints = new();
    private const int Margin = 60;
    private const int AxisMargin = 40;

    public void SetData(List<(int x, int y)> data)
    {
        _dataPoints = data;
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (_dataPoints.Count == 0)
        {
            DrawNoDataMessage(context);
            return;
        }

        DrawChart(context);
    }

    private void DrawNoDataMessage(DrawingContext context)
    {
        var text = new FormattedText(
            "Немає даних для відображення",
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Arial"),
            16,
            Brushes.Gray);

        context.DrawText(text, new Avalonia.Point(
            (Bounds.Width - text.Width) / 2,
            (Bounds.Height - text.Height) / 2));
    }

    private void DrawChart(DrawingContext context)
    {
        double width = Bounds.Width;
        double height = Bounds.Height;

        if (width < 100 || height < 100) return;

        // Визначаємо діапазони даних
        int xMin = _dataPoints.Min(p => p.x);
        int xMax = _dataPoints.Max(p => p.x);
        int yMin = _dataPoints.Min(p => p.y);
        int yMax = _dataPoints.Max(p => p.y);

        // Розширюємо діапазон на 10% для кращого вигляду
        int xRange = xMax - xMin;
        int yRange = yMax - yMin;
        xMin -= xRange / 10;
        xMax += xRange / 10;
        yMin -= yRange / 10;
        yMax += yRange / 10;

        // Робоча область для графіка
        double chartWidth = width - 2 * Margin;
        double chartHeight = height - 2 * Margin;

        // Малюємо осі
        DrawAxes(context, width, height, chartWidth, chartHeight, xMin, xMax, yMin, yMax);

        // Малюємо графік
        DrawGraph(context, chartWidth, chartHeight, xMin, xMax, yMin, yMax);
    }

    private void DrawAxes(DrawingContext context, double width, double height, 
        double chartWidth, double chartHeight, int xMin, int xMax, int yMin, int yMax)
    {
        var axisPen = new Pen(Brushes.Black, 2);
        var gridPen = new Pen(Brushes.LightGray, 1);

        // Вісь X
        context.DrawLine(axisPen,
            new Avalonia.Point(Margin, height - Margin),
            new Avalonia.Point(width - Margin, height - Margin));

        // Вісь Y
        context.DrawLine(axisPen,
            new Avalonia.Point(Margin, Margin),
            new Avalonia.Point(Margin, height - Margin));

        // Стрілка на осі X
        context.DrawLine(axisPen,
            new Avalonia.Point(width - Margin, height - Margin),
            new Avalonia.Point(width - Margin - 10, height - Margin - 5));
        context.DrawLine(axisPen,
            new Avalonia.Point(width - Margin, height - Margin),
            new Avalonia.Point(width - Margin - 10, height - Margin + 5));

        // Стрілка на осі Y
        context.DrawLine(axisPen,
            new Avalonia.Point(Margin, Margin),
            new Avalonia.Point(Margin - 5, Margin + 10));
        context.DrawLine(axisPen,
            new Avalonia.Point(Margin, Margin),
            new Avalonia.Point(Margin + 5, Margin + 10));

        // Підписи осей
        DrawAxisLabel(context, "X", width - Margin + 15, height - Margin + 5);
        DrawAxisLabel(context, "Y", Margin - 20, Margin - 10);

        // Розмітка осі X
        int xStep = Math.Max(1, (xMax - xMin) / 10);
        for (int x = xMin; x <= xMax; x += xStep)
        {
            double px = Margin + (x - xMin) * chartWidth / (xMax - xMin);
            
            // Вертикальна сітка
            context.DrawLine(gridPen,
                new Avalonia.Point(px, Margin),
                new Avalonia.Point(px, height - Margin));
            
            // Значення
            DrawAxisLabel(context, x.ToString(), px - 10, height - Margin + 10);
        }

        // Розмітка осі Y
        int yStep = Math.Max(1, (yMax - yMin) / 10);
        for (int y = yMin; y <= yMax; y += yStep)
        {
            double py = height - Margin - (y - yMin) * chartHeight / (yMax - yMin);
            
            // Горизонтальна сітка
            context.DrawLine(gridPen,
                new Avalonia.Point(Margin, py),
                new Avalonia.Point(width - Margin, py));
            
            // Значення
            DrawAxisLabel(context, y.ToString(), Margin - 35, py - 8);
        }
    }

    private void DrawAxisLabel(DrawingContext context, string text, double x, double y)
    {
        var formattedText = new FormattedText(
            text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Arial"),
            12,
            Brushes.Black);

        context.DrawText(formattedText, new Avalonia.Point(x, y));
    }

    private void DrawGraph(DrawingContext context, double chartWidth, double chartHeight,
        int xMin, int xMax, int yMin, int yMax)
    {
        var linePen = new Pen(Brushes.Blue, 2);
        var pointBrush = Brushes.Red;

        for (int i = 0; i < _dataPoints.Count; i++)
        {
            var point = _dataPoints[i];
            
            double px = Margin + (point.x - xMin) * chartWidth / (xMax - xMin);
            double py = Bounds.Height - Margin - (point.y - yMin) * chartHeight / (yMax - yMin);

            // Малюємо точку
            context.DrawEllipse(pointBrush, null, new Avalonia.Point(px, py), 4, 4);

            // Малюємо лінію до наступної точки
            if (i < _dataPoints.Count - 1)
            {
                var nextPoint = _dataPoints[i + 1];
                double npx = Margin + (nextPoint.x - xMin) * chartWidth / (xMax - xMin);
                double npy = Bounds.Height - Margin - (nextPoint.y - yMin) * chartHeight / (yMax - yMin);
                
                context.DrawLine(linePen, new Avalonia.Point(px, py), new Avalonia.Point(npx, npy));
            }
        }
    }
}
