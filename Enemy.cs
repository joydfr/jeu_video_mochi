using Godot;
using System;
public partial class Enemy : CharacterBody2D
{
	[Export] public float detectionRange = 150.0f;
	public const float Speed = 150.0f;
	public const float Gravity = 980.0f;
	private AnimatedSprite2D charAnim;
	private CharacterBody2D player;
	private Vector2 pointA;
	private Vector2 pointB;
	private bool movingToA = true;
	private bool playerWasDetected = false;
	public override void _Ready()
	{
		charAnim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		player = GetNode<CharacterBody2D>("../Player");
		
		var markerA = GetNodeOrNull<Marker2D>("PointA");
		var markerB = GetNodeOrNull<Marker2D>("PointB");
		
		if (markerA != null && markerB != null)
		{
			pointA = new Vector2(markerA.GlobalPosition.X, GlobalPosition.Y);
			pointB = new Vector2(markerB.GlobalPosition.X, GlobalPosition.Y);
			GD.Print($"Points de patrouille: A:{pointA}, B:{pointB}");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		
		// Appliquer la gravité
		if (!IsOnFloor())
		{
			velocity.Y += Gravity * (float)delta;
		}
		else 
		{
			velocity.Y = 0;
		}
		
		
		if (player != null)
		{
			float distanceToPlayer = GlobalPosition.DistanceTo(player.GlobalPosition);
			float heightDifference = Mathf.Abs(player.GlobalPosition.Y - GlobalPosition.Y);
			
			// Vérifier si le joueur est à la même hauteur (avec une marge de tolérance)
			bool isPlayerAbove = player.GlobalPosition.Y < GlobalPosition.Y;
			bool isPlayerOnSameLevel = heightDifference < 100 && isPlayerAbove;// Ajustez cette valeur selon vos besoins
			
			if (distanceToPlayer < detectionRange && isPlayerOnSameLevel)
			{
				playerWasDetected = true;
				float direction = Mathf.Sign(player.GlobalPosition.X - GlobalPosition.X);
				velocity.X = direction * Speed;
				
				charAnim.Play("walk");
				charAnim.SpeedScale = 2.0f;
				charAnim.FlipH = velocity.X < 0;
			}
			else if (playerWasDetected && distanceToPlayer >= detectionRange)
			{
				// Le joueur vient de sortir de la zone de détection
				playerWasDetected = false;
				velocity.X = 0;
				charAnim.Play("Idle");
				charAnim.SpeedScale = 1.0f;
			}
			else
			{
				// Pas de joueur détecté, rester immobile
				velocity.X = 0;
				charAnim.Play("Idle");
				charAnim.SpeedScale = 1.0f;
			}
		}
		
		Velocity = velocity;
		MoveAndSlide();
	}
}
