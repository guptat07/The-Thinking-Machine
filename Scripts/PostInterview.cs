using Godot;
using System;

public partial class PostInterview : Node2D
{
    [Export] public NodePath DialogicSingletonPath = "/root/Dialogic";
	[Export] public string DialoguePathCheat = "res://Assets/Dialogue/interview_cheated.dtl";
    [Export] public string DialoguePathLegit = "res://Assets/Dialogue/interview_legit.dtl";
    private Node dlg;
    private SceneBg scenebg;
    public override void _Ready()
    {
        scenebg = GetNodeOrNull<SceneBg>("CanvasLayer");
        if(scenebg == null) {
            GD.Print("CANVAS LAYER BUGGED");
            return;
        }
        dlg = GetNodeOrNull<Node>(DialogicSingletonPath);
        if(dlg == null){
            GD.Print("dlg broken");
            return;
        }
        dlg.Connect("signal_event", new Callable(this, nameof(OnDialogicSignal)));
        if(ArduinoUDP.Instance.justUsed){
            ArduinoUDP.Instance.justUsed = false;
            scenebg.TextureName = "cheated.png";
            dlg.Call("start",DialoguePathCheat);
        }  else {
            scenebg.TextureName = "dont_cheat_interview.png";
            dlg.Call("start",DialoguePathLegit);
        }
    }
    private void  OnDialogicSignal(Variant argument){
        if (argument.AsString() == "switch_scene") {
            GetTree().ChangeSceneToFile("res://Scenes/junior_dev.tscn");
        }
    }
}
