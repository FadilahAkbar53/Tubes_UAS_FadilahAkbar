using Godot;
using System;

public partial class ObstacleDamage : Area3D
{
    [Export] public int DamageAmount = 10;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        GD.Print("Entered: " + body.Name); // debug

        if (body is Player1 player)
        {
            player.TakeDamage(DamageAmount);
        }
    }
}
