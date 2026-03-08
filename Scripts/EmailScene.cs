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
            // Send 32-char padded string to Arduino, receive boolean response
            _ = ArduinoUDP.SendAndReceiveAsync("i_can_help").ContinueWith(task => {
                if (task.IsFaulted)
                    GD.PrintErr($"[EmailScene] Arduino error: {task.Exception?.Message}");
                else
                    GD.Print($"[EmailScene] Arduino responded: {task.Result}");
            });
        }
    }
    private void OnDialogicTimelineEnded()
    {
        GD.Print("End of dialogue");
        GetTree().ChangeSceneToFile("res://Scenes/assignment_scene.tscn");
    }
}
