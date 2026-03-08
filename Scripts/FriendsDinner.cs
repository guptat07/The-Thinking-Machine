using Godot;
using System;

public partial class FriendsDinner : Node2D
{
    [Export] public NodePath DialogicSingletonPath = "/root/Dialogic";
    [Export] public string DialoguePathCheat = "res://Assets/Dialogue/friends_dinner.dtl";
    [Export] public string DialoguePathLegit = "res://Assets/Dialogue/passed_over.dtl";
    [Export] public string jobProjLegit = "res://Assets/Dialogue/job_project_legit.dtl";
    [Export] public string peerWorkLegit = "res://Assets/Dialogue/peer_work_legit.dtl";
    private Node dlg;
    public override void _Ready()
    {
        dlg = GetNodeOrNull<Node>(DialogicSingletonPath);
        if(dlg == null){
            GD.Print("dlg broken");
            return;
        }

        dlg.Connect("signal_event", new Callable(this, nameof(OnDialogicSignal)));
        if(ArduinoUDP.Instance.justUsed) {
            ArduinoUDP.Instance.justUsed = false;
            dlg.Call("start",DialoguePathCheat);
        } else {
            dlg.Call("start",DialoguePathLegit);
        }
    }

    private async void  OnDialogicSignal(Variant argument)
    {
        GD.Print($"Dialogic Signal Event reached! Argument: {argument.ToString()}");
        if(argument.AsString() == "i_remember"){
            // Send 32-char padded string to Arduino, receive boolean response
            bool raised = await ArduinoUDP.Instance.SendAndReceiveAsync("I remember everything.");
            if(raised) {
                GD.Print("True");
                ArduinoUDP.Instance.tm_uses += 1;
                ArduinoUDP.Instance.justUsed = true;
                GetTree().ChangeSceneToFile("res://Scenes/dad.tscn");
            } else {
                GD.Print("False");

            }
        } else if (argument.AsString() == "continue") {
            dlg.Call("start", jobProjLegit);
        } else if (argument.AsString() == "do_it_for") {
            bool raised = await ArduinoUDP.Instance.SendAndReceiveAsync("I can do the work for you.");
            if(raised) {
                GD.Print("True");
                ArduinoUDP.Instance.tm_uses += 1;
                ArduinoUDP.Instance.justUsed = true;
            } else {
                GD.Print("False");
                ArduinoUDP.Instance.nos += 1;
                GetTree().ChangeSceneToFile("res://Scenes/.tscn");
            }
        } else if (argument.AsString() == "okay_or_good_friend") {
            if(ArduinoUDP.Instance.tm_uses > 0) {
                GetTree().ChangeSceneToFile("res://Scenes/friends_good.tscn");
            } else {
                GetTree().ChangeSceneToFile("res://Scenes/fired.tscn");
            }
        }
    }
}
