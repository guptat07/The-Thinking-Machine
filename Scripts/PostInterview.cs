using Godot;
using System;

public partial class PostInterview : Node2D
{
    [Export] public NodePath DialogicSingletonPath = "/root/Dialogic";
	[Export] public string DialoguePathCheat = "res://Assets/Dialogue/interview_cheat.dtl";
    [Export] public string DialoguePathLegit = "res://Assets/Dialogue/interview_legit.dtl";

    public override void _Ready()
    {
        var dlg = GetNodeOrNull<Node>(DialogicSingletonPath);
        if(dlg == null){
            GD.Print("dlg broken");
            return;
        }
        dlg.Connect("timeline_ended", new Callable(this, nameof(OnDialogicTimelineEnded)));
        if(ArduinoUDP.Instance.justUsed){
            ArduinoUDP.Instance.justUsed = false;
            dlg.Call("start",DialoguePathCheat);
        } else {
            dlg.Call("start",DialoguePathLegit);
        }
    }
    private void OnDialogicTimelineEnded()
    {
        GD.Print("End of dialogue");
        GetTree().ChangeSceneToFile("res://Scenes/junior_dev.tscn");
    }
   
}
