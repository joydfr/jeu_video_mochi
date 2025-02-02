using Godot;
using System;


public partial class Doggy : CharacterBody2D // Assure-toi que tu utilises bien CharacterBody2D
{
    [Export] public int maxHealth = 100; // Santé maximale de l'ennemi
    private int currentHealth;

    [Export] public float speed = 100f; // Vitesse de l'ennemi
    private Vector2 targetPosition; // Cible actuelle (Point A ou Point B)
    private bool movingToA = true; // Savoir si on se déplace vers A ou B
    private AnimatedSprite2D charAnim;

    // Référence vers les points A et B
    private Marker2D pointA;
    private Marker2D pointB;

    private Vector2 velocity = Vector2.Zero; // Vitesse de l'ennemi


    // Fonction appelée lorsque l'ennemi est prêt
    public override void _Ready()
    {
        currentHealth = maxHealth;

        // Obtenir les positions des points A et B
        pointA = GetNode<Marker2D>("PointA");
        pointB = GetNode<Marker2D>("PointB");

        // Commencer en se déplaçant vers le point A
        targetPosition = pointA.Position;

        GD.Print("Enemy Ready. Starting at position: " + Position);
        GD.Print("Target position is: " + targetPosition);

        charAnim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }


    // Fonction appelée à chaque frame physique
    public override void _PhysicsProcess(double delta)
    {
        // Déplacer l'ennemi vers la cible
        MoveToTarget(delta);


        if (IsOnFloor())
        {
            charAnim.Play("walk");
        }
    }


    // Déplacer l'ennemi vers la cible
    private void MoveToTarget(double delta)
    {
        // Calculer la direction vers la cible
        Vector2 direction = (targetPosition - Position).Normalized();

        // Ajuster la vélocité en fonction de la direction et de la vitesse
        velocity = direction * speed;

        // s'assure qu'il y a pas de mouvement de haut en bas
        Velocity = new Vector2(velocity.X, Velocity.Y);

        // fait le déplacement grace a velocity
        MoveAndSlide();

        GD.Print("Enemy Position: " + Position + ", Target: " + targetPosition);

        // Le changement de direction est immédiat, donc on n'a pas besoin de vérifier la proximité
        // Alterner entre PointA et PointB immédiatement
        if (Position.X >= targetPosition.X - 5f && Position.X <= targetPosition.X + 5f)
        {
            // Si l'ennemi a atteint le point, on change la cible instantanément
            movingToA = !movingToA;
            targetPosition = movingToA ? pointA.Position : pointB.Position;

            GD.Print("New target: " + targetPosition);
        }
    }

    public void _on_area_2d_body_entered(Node body)
    {
        if (!IsOnFloor() && velocity.Y > 0)  // Si on est sur le sol et qu'on descend
        {
            velocity.Y = -velocity.Y * 0.5f;  // Rebondir (inverser la vitesse en Y)
        }
    }

    public void AttaqueChien(Node body)
    {
        // si le joueur entre dans cette zone le joueur meurt 
        if (body is Player player)
        {
            player.QueueFree();
            GD.Print("Le Joueur est mort");
        }
    }
}