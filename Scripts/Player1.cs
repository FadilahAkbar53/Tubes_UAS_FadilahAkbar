using Godot;
using System;

public partial class Player1 : CharacterBody3D
{
    [Export] public float MoveSpeed = 5.0f;
    [Export] public float JumpForce = 10.0f;
    [Export] public float Gravity = 20.0f;
    
    [Export] public NodePath CameraRigPath;

    private Node3D _cameraRig;
    private AnimationPlayer _animPlayer;

    private Vector3 _velocity = Vector3.Zero;
    private bool _wasOnFloorLastFrame = true;
    private bool _isDead = false;

    public override void _Ready()
    {
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        if (CameraRigPath != null)
            _cameraRig = GetNode<Node3D>(CameraRigPath);
        else
            GD.PushError("CameraRigPath belum diisi di Inspector!");

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isDead || _cameraRig == null)
            return;

        Vector3 inputDir = GetInputDirection();
        Vector3 moveDir = GetMoveDirection(inputDir).Normalized();
        bool isMoving = moveDir.LengthSquared() > 0.01f;
        bool isOnGround = IsOnFloor();

        // Gerakan horizontal
        Vector3 horizontalVelocity = moveDir * MoveSpeed;
        _velocity.X = horizontalVelocity.X;
        _velocity.Z = horizontalVelocity.Z;

        // Rotasi karakter ke arah gerak (jika bergerak)
        if (isMoving)
        {
            Vector3 lookDir = moveDir.Normalized();
            LookAt(Position - lookDir, Vector3.Up);
        }

        // Gravity
        if (!isOnGround)
        {
            _velocity.Y -= Gravity * (float)delta;
        }

        // Jump
        if (isOnGround && Input.IsActionJustPressed("jump"))
        {
            _velocity.Y = JumpForce;
            PlayAnim("jump");
        }

        // Apply movement
        Velocity = _velocity;
        MoveAndSlide();
        _velocity = Velocity;

        // Transisi animasi otomatis
        if (!_wasOnFloorLastFrame && isOnGround)
        {
            PlayAnim(isMoving ? "running" : "idle");
        }
        else if (isOnGround && !Input.IsActionJustPressed("jump"))
        {
            PlayAnim(isMoving ? "running" : "idle");
        }

        _wasOnFloorLastFrame = isOnGround;
    }

    private Vector3 GetInputDirection()
    {
        Vector3 dir = Vector3.Zero;

        if (Input.IsActionPressed("forward")) dir.Z += 1;
        if (Input.IsActionPressed("backward")) dir.Z -= 1;
        if (Input.IsActionPressed("left")) dir.X -= 1;
        if (Input.IsActionPressed("right")) dir.X += 1;

        return dir;
    }

    private Vector3 GetMoveDirection(Vector3 inputDir)
    {
        Basis camBasis = _cameraRig.GlobalTransform.Basis;

        Vector3 camForward = -camBasis.Z;
        camForward.Y = 0;
        camForward = camForward.Normalized();

        Vector3 camRight = camBasis.X;
        camRight.Y = 0;
        camRight = camRight.Normalized();

        return (camRight * inputDir.X + camForward * inputDir.Z);
    }

    private void PlayAnim(string name)
    {
        if (_animPlayer.IsPlaying() && _animPlayer.CurrentAnimation == name) return;
        _animPlayer.Play(name);
    }

    public void Die()
    {
        if (_isDead) return;

        _isDead = true;
        _velocity = Vector3.Zero;
        Velocity = Vector3.Zero;

        PlayAnim("die");
        GD.Print("Player is dead.");
    }
}
