using Godot;
using System;



public partial class Player : CharacterBody2D
{
    public const float Speed = 250.0f; // vitesse 
    public const float JumpVelocity = -250.0f; // saut 
    public int jump_Count = 0; //compteur des saut
    private AnimatedSprite2D charAnim;
    private AnimationPlayer animPlayer;


    public void Death()
    {
        GD.Print("Le personnage est mort et sera supprimé.");
        QueueFree();
    }



    public static void AttaqueGriffe(Node body)
    {
        // si le chien entre dans cette zone il meurt
        if (body is Doggy doggy) // Vérifie si le corps est de type Doggy
        {
            doggy.QueueFree();
            GD.Print("Le chien est mort");
        }
    }


    public override void _Ready()
    {
        charAnim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
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
            charAnim.Play("jump");
        }
        // Réinitialiser le compteur de sauts si le personnage touche le sol
        if (IsOnFloor())
        {
            jump_Count = 0;
        }

        // Flip l'animation selon la direction (gauche/droite).
        if (velocity.X != 0)
        {
            charAnim.FlipH = velocity.X < 0; // Inverser l'animation si on va à gauche
        }


        // la direction du personnage 
        Vector2 direction = new Vector2(
           Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
           Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
       );
        bool isAttacking = true;
        isAttacking = charAnim.Animation == "attack";

        // Si le personnage n'est pas en train d'attaquer
        if (!isAttacking)
        {
            if (direction != Vector2.Zero)
            {
                // Ajuster la vitesse en X et Y pour le mouvement
                velocity.X = direction.X * Speed;

                // la marche 
                charAnim.Play("walk");
            }
            else
            {
                // permet l'arret du déplacement
                velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);

                // si le personnage est a l'arret lance l'animation iddle 
                if (IsOnFloor() && velocity.X == 0 && velocity.Y == 0)
                {
                    charAnim.Play("idle");
                }
            }
        }
        if (!IsOnFloor() && Input.IsActionJustPressed("attaqueSauter"))
        {
            charAnim.Play("attackSauter");
            if (charAnim.FlipH == false)
            {
                GetNode<AnimationPlayer>("AnimationPlayer").Play("attaqueSauter");
            }
            else
            {
                GetNode<AnimationPlayer>("AnimationPlayer").Play("ReverseSauter");
            }
        }
        // l'attaque 
        if (Input.IsActionJustPressed("attack"))
        {
            charAnim.Play("attack");
            if (charAnim.FlipH == false)
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

}
