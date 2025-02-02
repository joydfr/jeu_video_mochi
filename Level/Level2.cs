using Godot;
using System;

public partial class Level2 : Node2D
{

	// ma variable 
	 private AnimationPlayer _animationPlayer;

	public override void _Ready()
{
	// appel de node AnimationPlayer que je peut rename en ce que je veut et qui va me permettre 
	// de faire plusieur plateform qui bouge 
	_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	//Et du coup execute l'animation Voulue.
		_animationPlayer.Play("MoveFirstPlateform"); 
}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
