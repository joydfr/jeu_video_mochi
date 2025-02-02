code ennemie
using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public float detectionRange = 300.0f;
	public const float Speed = 150.0f;
	public const float Gravity = 980.0f;
	private AnimatedSprite2D charAnim;
	private CharacterBody2D player;
	private Vector2 pointA;
	private Vector2 pointB;
	private bool movingToA = true;
	private bool playerWasDetected = false;
	private bool isDead = false;
	
	// Collision pour détecter si le joueur saute sur l'ennemi
	private Area2D deathArea;

	// Timer pour gérer la suppression après l'animation
	private Timer deathTimer;

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
		}

		// Initialisation de la zone de mort (collision) avec le nom "Died"
		deathArea = GetNode<Area2D>("Died");
		deathArea.BodyEntered += OnPlayerJumpOnEnemy;

		// Initialisation du timer pour la suppression après l'animation
		deathTimer = new Timer();
		deathTimer.OneShot = true;  // Il ne s'exécutera qu'une seule fois
		deathTimer.WaitTime = 1.0f; // Durée d'attente (en secondes) avant de supprimer l'ennemi
		AddChild(deathTimer);  // Ajouter le Timer à la scène
		deathTimer.Timeout += OnDeathTimerTimeout; // Lier l'événement Timeout
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (!isDead)
		{
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

				bool isPlayerInVerticalRange = heightDifference < 200;
				bool isPlayerInHorizontalRange = Mathf.Abs(player.GlobalPosition.X - GlobalPosition.X) < detectionRange;

				if (isPlayerInVerticalRange && isPlayerInHorizontalRange)
				{
					playerWasDetected = true;
					float direction = Mathf.Sign(player.GlobalPosition.X - GlobalPosition.X);
					velocity.X = direction * Speed;

					charAnim.Play("walk");
					charAnim.SpeedScale = 2.0f;
					charAnim.FlipH = velocity.X < 0;
				}
				else if (playerWasDetected && !isPlayerInHorizontalRange)
				{
					playerWasDetected = false;
					velocity.X = 0;
					charAnim.Play("Idle");
					charAnim.SpeedScale = 1.0f;
				}
				else
				{
					velocity.X = 0;
					charAnim.Play("Idle");
					charAnim.SpeedScale = 1.0f;
				}
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	// Fonction pour gérer l'impact lorsque le joueur saute sur l'ennemi
	private void OnPlayerJumpOnEnemy(Node body)
	{
		if (body is CharacterBody2D && !isDead)
		{
			// Vérifier que le joueur tombe sur l'ennemi (direction de la vitesse Y)
			CharacterBody2D player = body as CharacterBody2D;
			if (player.Velocity.Y > 0) // Le joueur tombe
			{
				// L'ennemi meurt
				isDead = true;
				charAnim.Play("Died"); // Si vous avez une animation de mort
				deathTimer.Start();  // Démarrer le timer avant de supprimer l'ennemi
			}
		}
	}

	// Lorsque le timer expire, supprimer l'ennemi
	private void OnDeathTimerTimeout()
	{
		QueueFree();  // Supprimer l'ennemi de la scène après le délai
	}
