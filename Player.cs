using Godot;
using System;


public partial class Player : CharacterBody2D
{
    public const float Speed = 250.0f; // vitesse 
    public const float JumpVelocity = -250.0f; // saut 
    public int life = 100;
    public int jump_Count = 0; //compteur des saut
    private AnimatedSprite2D Animation;
    private AnimationPlayer animPlayer;
    private Lifebar lifebar;

    public override void _Ready()
    {
        Animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        lifebar = GetNode<Lifebar>("../CanvasLayer/HUD");
        if (lifebar != null)
        {
            lifebar.MaxValue = life;
            lifebar.Value = life;

        }
    }

    public void AttaqueGriffe(Node body)
    {
        if (body is Doggy doggy)
        {
            doggy.QueueFree();
        }
        if (body is Enemy enemy)
        {
            enemy.QueueFree();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;


        // La gravité si le personnage est dans les air
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        // Gestion du saut limité a 1 car comence a 0 qui permet un double jump
        if (Input.IsActionJustPressed("ui_accept") && jump_Count < 1)
        {
            velocity.Y = JumpVelocity;
            jump_Count++;
            //Animation jump
            Animation.Play("jump");
        }
        // Réinitialiser le compteur de sauts si le personnage touche le sol
        if (IsOnFloor())
        {
            jump_Count = 0;
        }

        // Flip l'animation selon la direction (gauche/droite).
        if (velocity.X != 0)
        {
            Animation.FlipH = velocity.X < 0; // Inverser l'animation si on va à gauche
        }



        // la direction du personnage 
        Vector2 direction = new Vector2(
           Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
           Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
       );
        bool isAttacking = true;
        isAttacking = Animation.Animation == "attack";

        // Si le personnage n'est pas en train d'attaquer
        if (!isAttacking)
        {
            if (direction != Vector2.Zero)
            {
                // Ajuster la vitesse en X et Y pour le mouvement
                velocity.X = direction.X * Speed;
                if (IsOnFloor())
                {
                    Animation.Play("walk");
                }
                // la marche 

            }
            else
            {
                // permet l'arret du déplacement
                velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);

                // si le personnage est a l'arret lance l'animation iddle 
                if (IsOnFloor() && velocity.X == 0 && velocity.Y == 0)
                {
                    Animation.Play("idle");
                }
            }
        }
        // attaque sauter
        if (!IsOnFloor() && Input.IsActionJustPressed("attaqueSauter"))
        {
            var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
            sprite.Stop();
            Animation.Play("attackSauter");

            if (Animation.FlipH == false)
            {
                GetNode<AnimationPlayer>("AnimationPlayer").Play("attaqueSauter");
            }
            else
            {
                GetNode<AnimationPlayer>("AnimationPlayer").Play("ReverseSauter");
            }
            sprite.Play();
        }
        // l'attaque
        if (Input.IsActionJustPressed("attack"))
        {
            Animation.Play("attack");
            if (Animation.FlipH == false)
            {
                animPlayer.Play("attack");
            }
            else
            {
                animPlayer.Play("attackGauche");
            }

            isAttacking = false;
        }

        // Met à jour la vélocité et applique le mouvement.
        Velocity = velocity;
        MoveAndSlide();
    }

    public void TakeDamage(int degat)
    {
        life -= degat;
        Animation.Play("Hurt");
        GD.Print("Le joueur a été touché ! PV restants : " + life);

        if (lifebar != null)
        {
            lifebar.UpdateHealth(life);
        }
        if (life <= 0)
        {
            GD.Print("Le joueur est mort !");
            QueueFree();
        }
    }
}