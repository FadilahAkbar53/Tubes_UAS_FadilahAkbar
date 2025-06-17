using Godot;
using System;

public partial class ButtonExit : Button
{
    public override void _Pressed()
    {
        GetTree().Quit(); // Keluar dari game
    }
}
