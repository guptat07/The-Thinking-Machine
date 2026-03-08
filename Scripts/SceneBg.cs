using Godot;
using System;

[Tool]
public partial class SceneBg : CanvasLayer
{
    
    public TextureRect textureRect;
    public const string TexturePathBase = "res://Assets/Images/";
    
    private string _textureName = "";
    [Export] 
    public string TextureName 
    {
        get => _textureName;
        set
        {
            _textureName = value;
            UpdateTexture();
        }
    }

    public override void _Ready()
    {
        textureRect = GetNodeOrNull<TextureRect>("TextureRect");
        if(textureRect == null){
            GD.Print("textureRect broken");
            return;
        }
        UpdateTexture();
    }
    
    private void UpdateTexture()
    {
        // Don't try loading if we don't have a valid node or filename yet
        if (textureRect == null || string.IsNullOrWhiteSpace(_textureName)) return;

        string fullPath = TexturePathBase + _textureName;
        if (ResourceLoader.Exists(fullPath)) 
        {
            textureRect.Texture = GD.Load<Texture2D>(fullPath);
            GD.Print("AKSJDLKASJDLKJASLKDJASLKD");
        } 
        else 
        {
            GD.Print($"Failed to find texture at: {fullPath}");
        }
    }
}
