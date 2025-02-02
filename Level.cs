 using Godot;
using System;

public partial class Level : Node2D
{
	 private AnimationPlayer _animationPlayer; 
	[Export] public int damageAmount = 10;
	 // Montant des dégâts à infliger
	public override void _Ready()
	{
		  // appel de node AnimationPlayer que je peut rename en ce que je veut et qui va me permettre 
	// de faire plusieur plateform qui bouge 
	_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	//Et du coup execute l'animation Voulue.
		_animationPlayer.Play("droitegauche"); 
		
		
	}
	private void OnPlayerTouchSpikes(Node body)
	{
		if (body is Player player)
		{
			// Inflige des dégâts au joueur
			player.TakeDamage(damageAmount);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
