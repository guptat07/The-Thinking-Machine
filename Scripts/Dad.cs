using Godot;
using System;

public partial class Dad : Node2D
{
    [Export] public NodePath DialogicSingletonPath = "/root/Dialogic";
    [Export] public string DialoguePath = "res://Assets/Dialogue/senior_dev_cheat.dtl";
    public override void _Ready(){
    var dlg = GetNodeOrNull<Node>(DialogicSingletonPath);
        if(dlg == null){
            GD.Print("dlg broken");
            return;
        }

        dlg.Connect("signal_event", new Callable(this, nameof(OnDialogicSignal)));
        dlg.Call("start", DialoguePath);
    }
    private async void  OnDialogicSignal(Variant argument){
        if(argument.AsString() == "pick_up"){
            bool raised = false;
            while(!raised) {
                raised = await ArduinoUDP.Instance.SendAndReceiveAsync("Pick me up.");
                if(raised) {
                    GetTree().ChangeSceneToFile("res://Scenes/eulogy.tscn");
                    break;
                }
            }
        }
    }
}
