using Godot;
using System;

public partial class PostInterview : Node2D
{
    [Export] public NodePath DialogicSingletonPath = "/root/Dialogic";
	[Export] public string DialoguePathCheat = "res://Assets/Dialogue/interview_cheated.dtl";
    [Export] public string DialoguePathLegit = "res://Assets/Dialogue/interview_legit.dtl";
    private Node dlg;
    public override void _Ready()
    {
        dlg = GetNodeOrNull<Node>(DialogicSingletonPath);
        if(dlg == null){
            GD.Print("dlg broken");
            return;
        }
        dlg.Connect("signal_event", new Callable(this, nameof(OnDialogicSignal)));
        if(ArduinoUDP.Instance.justUsed){
            ArduinoUDP.Instance.justUsed = false;
            dlg.Call("start",DialoguePathCheat);
        }  else {
            dlg.Call("start",DialoguePathLegit);
        }
    }
    private void  OnDialogicSignal(Variant argument){
        if (argument.AsString() == "switch_scene") {
            GetTree().ChangeSceneToFile("res://Scenes/junior_dev.tscn");
        }
    }
}
