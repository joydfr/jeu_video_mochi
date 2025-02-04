using Godot;
using System;



public partial class Doggy : CharacterBody2D // Assure-toi que tu utilises bien CharacterBody2D
{
    [Export] public int lifeDog = 100;
    [Export] public float speed = 100f; // Vitesse de l'ennemi
    private Vector2 targetPosition; // Cible actuelle (Point A ou Point B)
    private bool movingToA = true; // Savoir si on se déplace vers A ou B
    private bool droite = true;
    private AnimatedSprite2D DoggyAnim;
    // Référence vers les points A et B
    private Marker2D pointA;
    private Marker2D pointB;
    private Vector2 velocity = Vector2.Zero; // Vitesse de l'ennemi
    private CollisionShape2D avant;
    private CollisionShape2D arriere;



    // Fonction appelée lorsque l'ennemi est prêt
    public override void _Ready()
    {
        Area2D area = GetNode<Area2D>("Area2D");
        // Obtenir les positions des points A et B
        pointA = GetNode<Marker2D>("PointA");
        pointB = GetNode<Marker2D>("PointB");
        // Commencer en se déplaçant vers le point A
        targetPosition = pointA.Position;
        DoggyAnim = GetNode<AnimatedSprite2D>("Sprite2D");
        avant = area.GetNode<CollisionShape2D>("Avant");
        arriere = area.GetNode<CollisionShape2D>("Arriere");
    }

    public override void _PhysicsProcess(double delta)
    {
        // Déplacer l'ennemi vers la cible
        MoveToTarget(delta);
        // l'ance l'anime walk
        DoggyAnim.Play("walk");
        // un boolean qui verifie si il se dirige vers A 
        if (movingToA)
        {
            // si le mob se dirige vers A le flip change 
            DoggyAnim.FlipH = true;
            // la colision shape change en fonction de la direction du mob
            avant.Disabled = true;
            arriere.Disabled = false;
        }
        else
        {
            DoggyAnim.FlipH = false;
            arriere.Disabled = true;
            avant.Disabled = false;
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


        // Le changement de direction est immédiat, donc on n'a pas besoin de vérifier la proximité
        // Alterner entre PointA et PointB immédiatement
        if (Position.X >= targetPosition.X - 5f && Position.X <= targetPosition.X + 5f)
        {
            // Si l'ennemi a atteint le point, on change la cible instantanément
            movingToA = !movingToA;
            targetPosition = movingToA ? pointA.Position : pointB.Position;
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
        if(body is Player player)
        {
            player.TakeDamage(15);
            
        }
    }
}