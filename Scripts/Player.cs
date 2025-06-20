using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
    public const float RotationVelocity = 3.5f;

    public override void _Ready()
    {
        GetNode<AnimationTree>("AnimationTree").Active = true;
    }

	public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        float turnStrength = Input.GetAxis("left", "right");
        float moveStrength = Input.GetAxis("forward", "backward");

        RotateY(Mathf.DegToRad(turnStrength * RotationVelocity * (float)delta));

        Vector3 direction = (Transform.Basis * new Vector3(0, 0, moveStrength)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
