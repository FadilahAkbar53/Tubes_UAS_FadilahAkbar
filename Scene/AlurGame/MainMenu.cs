using Godot;
using System;

public partial class MainMenu : Control
{
    public override void _Ready()
    {
        GetNode<Button>("VBoxContainer/StartButton").Pressed += OnStartPressed;
        GetNode<Button>("VBoxContainer/CreditButton").Pressed += OnCreditPressed;
        GetNode<Button>("VBoxContainer/HelpButton").Pressed += OnHelpPressed;
        GetNode<Button>("VBoxContainer/ExitButton").Pressed += OnExitPressed;
    }

    private void OnStartPressed()
    {
        GetTree().ChangeSceneToFile("res://Scene/AlurGame/Premis.tscn");
    }

    private void OnCreditPressed()
    {
        GetTree().ChangeSceneToFile("res://Scene/AlurGame/Credit.tscn");
    }

    private void OnHelpPressed()
    {
        GetTree().ChangeSceneToFile("res://Scene/AlurGame/Help.tscn");
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}
