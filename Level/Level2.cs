using Godot;
using System;




public partial class Level2 : Node2D
{
    // ma variable 
    public AnimationPlayer _animationPlayer;
    public AnimationPlayer trap;
    public AnimatedSprite2d animationMap;
    public Area2D area;



    public override void _Ready()
    {
        area = GetNode<Area2D>("ZonePortals");
        // appel de node AnimationPlayer que je peut rename en ce que je veut et qui va me permettre 
        // de faire plusieur plateform qui bouge  

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.Play("MoveFirstPlateform");
        trap = GetNode<AnimationPlayer>("trap");
        //Et du coup execute l'animation Voulue.
        trap.Play("Poussoir");

    }



    public void Teleport(CharacterBody2D body)
    {
        if (body is Player player)
        {
            GetTree().ChangeSceneToFile("res://level.tscn");
            GD.Print("t'es dans la position");
        }
    }


    public void Ecrase(CharacterBody2D body)
    {
        if (body is Player player)
        {
            player.TakeDamage(10);
        }
    }



    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}