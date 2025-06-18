using Godot;
using System;
using System.Collections.Generic;

public partial class DialogBox : Control
{
    private Label _dialogLabel;
    private Button _nextButton;

    private List<string> _lines = new();
    private int _currentLine = 0;

    public override void _Ready()
    {
        _dialogLabel = GetNode<Label>("DialogLabel");
        _nextButton = GetNode<Button>("NextButton");

        _nextButton.Pressed += OnNextPressed;
        Hide(); // Default tidak terlihat
    }

    public void StartDialog(List<string> lines)
    {
        _lines = lines;
        _currentLine = 0;
        Show();
        ShowLine();
    }

    private void ShowLine()
    {
        if (_currentLine < _lines.Count)
        {
            _dialogLabel.Text = _lines[_currentLine];
        }
        else
        {
            Hide(); // selesai dialog
        }
    }

    private void OnNextPressed()
    {
        _currentLine++;
        ShowLine();
    }
}
