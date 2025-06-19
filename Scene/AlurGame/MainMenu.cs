using Godot;
using System;

public partial class MainMenu : Control
{
    public override void _Ready()
    {
        GetNode<Button>("VBoxContainer/ButtonStart").Pressed += OnStartPressed;
        GetNode<Button>("VBoxContainer/ButtonCredit").Pressed += OnCreditPressed;
        GetNode<Button>("VBoxContainer/ButtonHelp").Pressed += OnHelpPressed;
        GetNode<Button>("VBoxContainer/ButtonExit").Pressed += OnExitPressed;
    }

    private void OnStartPressed()
    {
        GetTree().ChangeSceneToFile("res://Scene/AlurGame/premis.tscn");
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
