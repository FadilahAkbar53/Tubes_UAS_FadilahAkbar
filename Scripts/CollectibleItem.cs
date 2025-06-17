using Godot;
using System;

public partial class CollectibleItem : Area3D
{
    [Export] public string ItemName = "Unknown Item";
    [Export] public Texture2D ItemIcon;

    public bool PlayerInArea = false;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is Player1 player)
        {
            PlayerInArea = true;
            player.SetNearbyItem(this);
        }
    }


    private void OnBodyExited(Node3D body)
    {
        if (body is Player1)
        {
            PlayerInArea = false;
        }
    }

    public void Collect()
    {
        GD.Print($"📦 Collected: {ItemName}");
        QueueFree(); // hilangkan item dari scene
    }
}
