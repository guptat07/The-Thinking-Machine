using Godot;
using System;

public partial class Fired : Node2D
{
    [Export] public NodePath DialogicSingletonPath = "/root/Dialogic";
	[Export] public string DialoguePath = "res://Assets/Dialogue/fired.dtl";
    public override void _Ready()
    {
        var dlg = GetNodeOrNull<Node>(DialogicSingletonPath);
        if(dlg == null){
            GD.Print("dlg broken");
            return;
        }

        dlg.Connect("signal_event", new Callable(this, nameof(OnDialogicSignal)));

        dlg.Call("start", DialoguePath);
    }
    
    private async void  OnDialogicSignal(Variant argument)
    {
        GD.Print($"Dialogic Signal Event reached! Argument: {argument.ToString()}");
        if(argument.AsString() == "choose_ending"){
            // Send 32-char padded string to Arduino, receive boolean response
            GetTree().ChangeSceneToFile("res://Scenes/okay_bad_ending.tscn");
        }
    }
}
