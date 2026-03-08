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

        dlg.Connect("timeline_ended", new Callable(this, nameof(OnDialogicTimelineEnded)));
        dlg.Call("start", DialoguePath);
    }
    private void OnDialogicTimelineEnded()
    {
        GD.Print("End of dialogue");
        GetTree().ChangeSceneToFile("res://Scenes/email_scene.tscn");
    }
}
