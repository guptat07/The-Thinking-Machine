using Godot;
using System;

public partial class OkayBadEnding : Node2D
{
    [Export] public NodePath DialogicSingletonPath = "/root/Dialogic";
	[Export] public string OkEnding = "res://Assets/Dialogue/okay_ending.dtl";
	[Export] public string BadEnding = "res://Assets/Dialogue/bad_ending.dtl";
    public override void _Ready()
    {
        var dlg = GetNodeOrNull<Node>(DialogicSingletonPath);
        if(dlg == null){
            GD.Print("dlg broken");
            return;
        }

        if(ArduinoUDP.Instance.nos > ArduinoUDP.Instance.tm_uses){
            dlg.Call("start", OkEnding);
        }
        else {
            dlg.Call("start", BadEnding);
        }
    }
}
