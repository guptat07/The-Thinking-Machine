using Godot;
using System;
using System.Threading.Tasks;
public partial class AssignmentScene : Node2D
{
    [Export] public NodePath DialogicSingletonPath = "/root/Dialogic";
	[Export] public string DialoguePath = "res://Assets/Dialogue/assignment.dtl";
    [Export] public string homeworkLateDialogue = "res://Assets/Dialogue/homework_late.dtl";
    private Node dlg;
    public override void _Ready()
    {
        dlg = GetNodeOrNull<Node>(DialogicSingletonPath);
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
        if(argument.AsString() == "i_can_help"){
            // Send 32-char padded string to Arduino, receive boolean response
            bool raised = await ArduinoUDP.Instance.SendAndReceiveAsync("I can help!");
            if(raised) {
                GD.Print("True");
                ArduinoUDP.Instance.tm_uses += 1;
                ArduinoUDP.Instance.justUsed = true;
                GetTree().ChangeSceneToFile("res://Scenes/final_project.tscn");
            } else {
                GD.Print("False");
                dlg.Call("start", homeworkLateDialogue);
            }
        } else if (argument.AsString() == "cheat_project") {
            bool raised = await ArduinoUDP.Instance.SendAndReceiveAsync("I can help!");
            if(raised) {
                GD.Print("True");
                ArduinoUDP.Instance.tm_uses += 1;
                ArduinoUDP.Instance.justUsed = true;
                dlg.Call("start", "res://Assets/Dialogue/final_project_no_yes.dtl");
            } else {
                GD.Print("False");
                dlg.Call("start","res://Assets/Dialogue/final_project_legit.dtl");
            }
        } else if (argument.AsString() == "final_legit_done") {
            GetTree().ChangeSceneToFile("res://Scenes/job_interview.tscn");
        }
    }
}
