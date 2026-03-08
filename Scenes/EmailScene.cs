using Godot;
using System;

public partial class EmailScene : Node2D
{
    [Export] public NodePath DialogicSingletonPath = "/root/Dialogic";
	[Export] public string DialoguePath = "res://Assets/Dialogue/email.dtl";
    public override void _Ready()
    {
        var dlg = GetNodeOrNull<Node>(DialogicSingletonPath);
        if(dlg == null){
            GD.Print("dlg broken");
            return;
        }

        // connects the emit signal in dialogic
        dlg.Connect("signal_event", new Callable(this, nameof(OnDialogicSignal)));
        dlg.Call("start", DialoguePath);
    }
    
    private void OnDialogicSignal(Variant argument)
    {
        GD.Print($"Dialogic Signal Event reached! Argument: {argument.ToString()}");
        if(argument.AsString() == "ich"){
            GD.Print("I can help!");
        }
    }
}
