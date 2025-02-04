using Godot;
using System;

public partial class MenuPrincipal : Control
{
    private PackedScene level;

    public override void _Ready()
    {
        // Charge la scène depuis le fichier .tscn
        level = (PackedScene)ResourceLoader.Load("res://Level/level_2.tscn");
    }

    public void Play()
    {
        // Change la scène en utilisant le PackedScene chargé
        GetTree().ChangeSceneToPacked(level);
    }

    public void Exit()
    {
        GetTree().Quit();
    }
}