using Godot;
using System;

public partial class test : Node
{
    [Export] public NodePath DialogicSingletonPath = "/root/Dialogic";
	[Export] public string DialoguePath = "res://Assets/Dialogue/test_timeline.dtl";

    public override void _Ready()
    {
        var dlg = GetNodeOrNull<Node>(DialogicSingletonPath);
        if(dlg == null){
            GD.Print("SKDFJLKS");
        }
        dlg.Call("start", DialoguePath);
    }
}
