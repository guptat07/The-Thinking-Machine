using Godot;
using System;

public partial class start : Node
{
    [Export] public NodePath DialogicSingletonPath = "/root/Dialogic";
	[Export] public string DialoguePath = "res://Assets/Dialogue/start.dtl";
    public override void _Ready()
    {
        var dlg = GetNodeOrNull<Node>(DialogicSingletonPath);
        if(dlg == null){
            GD.Print("dlg broken");
            return;
        }

        // dlg.Connect("signal_event", new Callable(this, nameof(OnDialogicSignal)));
        dlg.Connect("timeline_ended", new Callable(this, nameof(OnDialogicTimelineEnded)));
        dlg.Call("start", DialoguePath);
    }
    
    // private void OnDialogicSignal(Variant argument)
    // {
    //     GD.Print($"Dialogic Signal Event reached! Argument: {argument.ToString()}");
    //     if(argument.AsString() == "ich"){
    //         // send signal to arduino to send text "I can help!"
    //     } else if (argument.AsString() == "let_me") {
    //         // send signal to arduino to send text ...
    //     }
    // }
    private void OnDialogicTimelineEnded()
    {
        GD.Print("End of dialogue");
        GetTree().ChangeSceneToFile("res://Scenes/email_scene.tscn");
    }
}
