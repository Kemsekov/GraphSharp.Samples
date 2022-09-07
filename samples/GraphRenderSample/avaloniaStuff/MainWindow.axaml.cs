using System.Linq;
using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;
using Avalonia;
using GraphSharp.Graphs;
using GraphSharp.GraphDrawer;
using GraphSharp;
using Avalonia.Media;
using System.Diagnostics;
using Avalonia.Input;

namespace Test;
public partial class MainWindow : Window
{
    event Action<KeyEventArgs> OnKeyDownEvent;
    public MainWindow()
    {
        InitializeComponent();
        var Canvas = this.FindControl<Canvas>("MyCanvas");
        var r = new Render(Canvas);
        OnKeyDownEvent = e=>{};
        RunRenderAfterAppActivation(r);
    }


    async void RunRenderAfterAppActivation(Render r){
        while(!IsActive) await Task.Delay(10);
        r.RenderStuff();
        r.ComputeStuff();
        OnKeyDownEvent += r.OnKeyDown;
    }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        OnKeyDownEvent.Invoke(e);
        base.OnKeyDown(e);
    }
}