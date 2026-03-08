using Godot;
using System;

public partial class EmailScene : Node
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

        dlg.Connect("signal_event", new Callable(this, nameof(OnDialogicSignal)));
        dlg.Connect("timeline_ended", new Callable(this, nameof(OnDialogicTimelineEnded)));
        dlg.Call("start", DialoguePath);
    }
    
    private void OnDialogicSignal(Variant argument)
    {
        GD.Print($"Dialogic Signal Event reached! Argument: {argument.ToString()}");
        if(argument.AsString() == "i_can_help"){
            // send text to arduino
        }
    }
    private void OnDialogicTimelineEnded()
    {
        GD.Print("End of dialogue");
        GetTree().ChangeSceneToFile("res://Scenes/assignment_scene.tscn");
    }
}
