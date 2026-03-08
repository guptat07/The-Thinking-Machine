using Godot;
using System;

public partial class start : Node
{
    [Export] public NodePath DialogicSingletonPath = "/root/Dialogic";
	[Export] public string DialoguePath = "res://Assets/Dialogue/";
    public string[] dialogueStems = {"start", "email", "assignment"};
    public string currentDialogue = "";
    int count = 0;
    public override void _Ready()
    {
        var dlg = GetNodeOrNull<Node>(DialogicSingletonPath);
        if(dlg == null){
            GD.Print("dlg broken");
            return;
        }

        // connects the emit signal in dialogic
        dlg.Connect("signal_event", new Callable(this, nameof(OnDialogicSignal)));
        dlg.Connect("timeline_ended", new Callable(this, nameof(OnDialogicTimelineEnded)));
        dlg.Call("start", DialoguePath + dialogueStems[count]+".dtl");
        count++;
    }
    
    private void OnDialogicSignal(Variant argument)
    {
        GD.Print($"Dialogic Signal Event reached! Argument: {argument.ToString()}");
        if(argument.AsString() == "ich"){
            GD.Print("I can help!");
        }
    }
    private void OnDialogicTimelineEnded()
    {
        if(count == dialogueStems.Length){
            GD.Print("End of dialogue");
        } else {
            currentDialogue = DialoguePath + dialogueStems[count]+".dtl";
            count++;
            GetNodeOrNull<Node>(DialogicSingletonPath).Call("start", currentDialogue);
        }
    }
}
