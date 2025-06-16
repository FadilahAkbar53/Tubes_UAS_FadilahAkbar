using Godot;
using System;

public partial class CameraFollow : Node3D
{
    [Export] public NodePath TargetPath;
    [Export] public float MouseSensitivity = 0.005f;
    [Export] public float FollowSpeed = 8f;
    [Export] public float PitchMin = -0.6f;
    [Export] public float PitchMax = 0.5f;

    private Node3D _target;
    private Node3D _springArm;

    private float _pitch = 0f;

    public override void _Ready()
    {
        if (TargetPath != null)
            _target = GetNode<Node3D>(TargetPath);
        else
            GD.PushError("CameraFollow: TargetPath belum di-set!");

        _springArm = GetNode<Node3D>("SpringArm3D");
    }

    public override void _Process(double delta)
    {
        if (_target == null)
            return;

        // Follow posisi target (player) dengan lerp biar smooth
        GlobalPosition = GlobalPosition.Lerp(_target.GlobalPosition, FollowSpeed * (float)delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            // Putar horizontal (Y)
            RotateY(-motion.Relative.X * MouseSensitivity);

            // Pitch vertikal
            _pitch = Mathf.Clamp(_pitch - motion.Relative.Y * MouseSensitivity, PitchMin, PitchMax);
            _springArm.Rotation = new Vector3(_pitch, 0, 0);
        }

        if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Escape)
        {
            Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured
                ? Input.MouseModeEnum.Visible
                : Input.MouseModeEnum.Captured;
        }
    }
}
