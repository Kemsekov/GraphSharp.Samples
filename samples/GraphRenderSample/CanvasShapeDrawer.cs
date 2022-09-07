using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;
using GraphSharp.Common;
using System.Drawing.Drawing2D;
namespace Test;
public class CanvasShapeDrawer : GraphSharp.GraphDrawer.IShapeDrawer
{
    ObjectPool<Line> lines;
    ObjectPool<Ellipse> ellipses;
    ObjectPool<TextBlock> textBlocks;
    public Canvas Canvas { get; }
    ConcurrentBag<IControl> toAdd;
    public CanvasShapeDrawer(Canvas canvas)
    {
        this.Canvas = canvas;
        toAdd = new();
        lines = new(() => new());
        ellipses = new(() => new());
        textBlocks = new(() => new());
    }

    public void ReturnToPool(object e){
        if(e is Line l) lines.Return(l);
        if(e is Ellipse el) ellipses.Return(el);
        if(e is TextBlock t) textBlocks.Return(t);
    }

    public void Dispatch()
    {
        foreach (var e in toAdd.Reverse()){
            ReturnToPool(e);
            if(Canvas.Children.Contains(e)) continue;
            Canvas.Children.Add(e);
        }
        toAdd.Clear();
    }

    IBrush ToBrush(System.Drawing.Color color)
    {
        return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B)); ;
    }

    void SetSize(Layoutable obj, float width, float height)
    {
        obj.Width = width;
        obj.Height = height;
    }
    void SetPosition(AvaloniaObject obj, Vector2 position)
    {
        
        var left = position.X;
        var top = position.Y;
        Canvas.SetLeft(obj, left);
        Canvas.SetTop(obj, top);
    }

    void SetColor(Shape shape, System.Drawing.Color color)
    {
        shape.Fill = ToBrush(color);
    }

    public void DrawText(string text, Vector2 position, System.Drawing.Color color, float fontSize)
    {
        var textBox = textBlocks.Get();
        textBox.FontSize = fontSize;
        textBox.Foreground = ToBrush(color);
        textBox.Text = text;
        SetPosition(textBox, position);
        toAdd.Add(textBox);
    }

    public void FillEllipse(Vector2 position, float width, float height, System.Drawing.Color color)
    {
        var el = ellipses.Get();
        position = new Vector2(position.X - width / 2, position.Y - height / 2);
        SetPosition(el, position);
        SetSize(el, width, height);
        SetColor(el, color);
        toAdd.Add(el);
    }

    public void DrawLine(Vector2 start, Vector2 end, System.Drawing.Color color, float thickness)
    {
        var line = lines.Get();
        line.StrokeThickness = thickness;
        line.Stroke = ToBrush(color);
        line.StartPoint = new(start.X, start.Y);
        line.EndPoint = new(end.X, end.Y);
        toAdd.Add(line);
    }

    public void Clear(System.Drawing.Color color)
    {
        Canvas.Children.Clear();
        Canvas.Background = ToBrush(color);
        toAdd.Clear();
    }
}