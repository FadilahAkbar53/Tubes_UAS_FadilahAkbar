using Godot;
using System;

public partial class Credit : Control
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<Button>("ButtonBack").Pressed += OnBackPressed;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.

    private void OnBackPressed()
    {
        GetTree().ChangeSceneToFile("res://Scene/AlurGame/MainMenu.tscn");
    }
}
