using Godot;
using System;
using System.Threading.Tasks;

public partial class JuniorDev : Node2D
{
    [Export] public NodePath DialogicSingletonPath = "/root/Dialogic";
	[Export] public string DialoguePath = "res://Assets/Dialogue/job_project.dtl";
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
        if(argument.AsString() == "can_you_do"){
            // Send 32-char padded string to Arduino, receive boolean response
            bool raised = await ArduinoUDP.Instance.SendAndReceiveAsync("Can you do this?");
            if(raised) {
                GD.Print("True");
                ArduinoUDP.Instance.tm_uses += 1;
                ArduinoUDP.Instance.justUsed = true;
            } else {
                GD.Print("False");
            }
            GetTree().ChangeSceneToFile("res://Scenes/friends_dinner.tscn");
        }
    }
}
