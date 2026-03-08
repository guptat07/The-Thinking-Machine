using Godot;
using System;

public partial class TitleScreen : Control
{
    public override void _Ready()
    {
        
    }
    public override void _Process(double delta)
    {
        if(Input.IsActionJustPressed("ui_accept")){
            GetTree().ChangeSceneToFile("res://Scenes/start_scene.tscn");
        }
    }
}
