using Godot;
using System;

public partial class ButtonRestart : Button
{
    public override void _Pressed()
    {
        GetTree().ReloadCurrentScene(); // Restart game
    }
}
