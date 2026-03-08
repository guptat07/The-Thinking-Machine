using Godot;
using System;

public partial class Eulogy : Node2D
{
    [Export] public NodePath DialogicSingletonPath = "/root/Dialogic";
    [Export] public string dad = "res://Assets/Dialogue/dad.dtl";
    [Export] public string eulogy = "res://Assets/Dialogue/eulogy.dtl";
    private Node dlg;
    public override void _Ready(){
    dlg = GetNodeOrNull<Node>(DialogicSingletonPath);
        if(dlg == null){
            GD.Print("dlg broken");
            return;
        }

        dlg.Connect("signal_event", new Callable(this, nameof(OnDialogicSignal)));
        dlg.Call("start", dad);
    }
    private async void  OnDialogicSignal(Variant argument){
        if (argument.AsString() == "dad_end"){
            dlg.Call("start", eulogy);            
        }
    }
}
