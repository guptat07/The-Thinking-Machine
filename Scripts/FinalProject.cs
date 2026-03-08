using Godot;
using System;
using System.Threading.Tasks;
public partial class FinalProject : Node2D
{
    [Export] public NodePath DialogicSingletonPath = "/root/Dialogic";
	[Export] public string DialoguePathCheat = "res://Assets/Dialogue/final_project_cheat.dtl";
    [Export] public string DialoguePathLegit = "res://Assets/Dialogue/final_project_legit.dtl";

    public override void _Ready()
    {
        var dlg = GetNodeOrNull<Node>(DialogicSingletonPath);
        if(dlg == null){
            GD.Print("dlg broken");
            return;
        }

        dlg.Connect("timeline_ended", new Callable(this, nameof(OnDialogicTimelineEnded)));
        GD.Print("TM USES: "+ArduinoUDP.Instance.tm_uses);
        if(ArduinoUDP.Instance.justUsed){
            dlg.Call("start", DialoguePathCheat);
            ArduinoUDP.Instance.justUsed = false;
        } else {
            dlg.Call("start", DialoguePathLegit);
        }
    }
    
    private void OnDialogicTimelineEnded()
    {
        GetTree().ChangeSceneToFile("res://Scenes/job_interview.tscn");
    }
}
