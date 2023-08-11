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
    public Render RenderApp { get; }

    event Action<KeyEventArgs> OnKeyDownEvent;
    public MainWindow()
    {
        InitializeComponent();
        var Canvas = this.FindControl<Canvas>("MyCanvas");
        this.RenderApp = new Render(Canvas);
        OnKeyDownEvent = e=>{};
        RunRenderAfterAppActivation();
    }


    async void RunRenderAfterAppActivation(){
        while(!IsActive) await Task.Delay(10);
        #pragma warning disable
        //this non-awaiting call is intended
        Task.Run(RenderApp.ComputeStuff);
        #pragma warning restore
        OnKeyDownEvent += RenderApp.OnKeyDown;
        RenderApp.RenderStuff();
    }
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        RenderApp.OnPointerPressed(e);
        base.OnPointerPressed(e);
    }
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        RenderApp.OnPointerWheelChanged(e);
        base.OnPointerWheelChanged(e);
    }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        OnKeyDownEvent.Invoke(e);
        base.OnKeyDown(e);
    }
}