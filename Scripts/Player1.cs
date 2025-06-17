using Godot;
using System;

public partial class Player1 : CharacterBody3D
{
    [Export] public float MoveSpeed = 5.0f;
    [Export] public float JumpForce = 10.0f;
    [Export] public float Gravity = 20.0f;
    [Export] public NodePath CameraRigPath;
    [Export] public int MaxHP = 100;
    [Export] public int RegenAmount = 5;
    [Export] public float RegenInterval = 5f;
    [Export] public NodePath GameOverUIPath;
    [Export] public NodePath SpringArmPath;
    [Export] public NodePath ItemInfoPanelPath;
    [Export] public NodePath InventoryNodePath;


    private SpringArm3D _springArm;
    private Control _gameOverUI;

    // Variabel health
    private int _currentHP;
    private float _regenTimer = 0f;
    private ProgressBar _healthBar;

    // Untuk kamera
    private Node3D _cameraRig;
    private AnimationPlayer _animPlayer;
    private Vector3 _velocity = Vector3.Zero;

    // Untuk animasi
    private bool _wasOnFloorLastFrame = true;
    private bool _isDead = false;

    // Untuk jatuh
    private float _fallStartY = 0f;
    private bool _wasInAir = false;
    private bool _playedFallJumpAnim = false;
    private bool _shouldDieAfterLanding = false;

    // Collect Item
    private CollectibleItem _currentItem = null;
    private Control _itemInfoPanel;
    private Label _itemInfoLabel;

    // Untuk inventory
    private SettingManager _inventory;

    public override void _Ready()
    {
        _currentHP = MaxHP;
        _healthBar = GetNode<ProgressBar>("../UI/HealthBar/HealthBar");
        _healthBar.MaxValue = MaxHP;
        _healthBar.Value = _currentHP;

        _gameOverUI = GetNode<Control>(GameOverUIPath);
        _gameOverUI.Visible = false; // Awal disembunyikan

        // Ambil referensi ke panel info item
        _itemInfoPanel = GetNode<Control>(ItemInfoPanelPath);
        _itemInfoLabel = _itemInfoPanel.GetNode<Label>("ItemNameLabel");
        _itemInfoPanel.Visible = false;

        // Ambil referensi ke inventory
        _inventory = GetNode<SettingManager>(InventoryNodePath);
        if (_inventory != null)
            GD.Print("✅ Inventory terhubung dengan benar!");
        else
            GD.PushError("❌ Gagal menemukan InventoryMono!");

        // Ambil referensi ke SpringArm
        if (SpringArmPath != null)
            _springArm = GetNode<SpringArm3D>(SpringArmPath);

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


        // Mulai jatuh
        if (!_wasInAir && !isOnGround)
        {
            _fallStartY = GlobalPosition.Y;
            _wasInAir = true;
            _playedFallJumpAnim = false;
        }

        // Di udara
        if (_wasInAir && !isOnGround && !_playedFallJumpAnim)
        {
            PlayAnim("jump"); // <-- animasi jatuh
            _playedFallJumpAnim = true;
        }

        // Mendarat
        if (_wasInAir && isOnGround)
        {
            float fallDistance = _fallStartY - GlobalPosition.Y;

            if (fallDistance >= 30f)
            {
                GD.Print("💀 Fall death from ", fallDistance, "m!");
                _shouldDieAfterLanding = true; // tandai untuk mati, jangan langsung
            }
            else if (fallDistance >= 20f)
            {
                GD.Print("💥 Fall damage 50HP from ", fallDistance, "m");
                TakeDamage(50);
            }
            else if (fallDistance >= 5f)
            {
                GD.Print("💥 Fall damage 5HP from ", fallDistance, "m");
                TakeDamage(5);
            }

            _wasInAir = false;
            _playedFallJumpAnim = false;
        }

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

        // Regenerasi HP tiap beberapa detik
        _regenTimer += (float)delta;
        if (_regenTimer >= RegenInterval && _currentHP < MaxHP)
        {
            _currentHP = Mathf.Min(_currentHP + RegenAmount, MaxHP);
            _healthBar.Value = _currentHP;
            _regenTimer = 0f;
        }

        if (_shouldDieAfterLanding && !_isDead)
        {
            _shouldDieAfterLanding = false;
            TakeDamage(_currentHP);
        }

        // Cek input collect item
        if (_currentItem != null && Input.IsActionJustPressed("collectItem"))
        {
            _inventory?.AddItem(_currentItem.ItemName);
            ShowItemPanel(_currentItem.ItemName);
            _currentItem.Collect();
            _currentItem = null;
        }
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

    public void TakeDamage(int amount)
    {
        if (_isDead) return;

        _currentHP -= amount;
        _currentHP = Mathf.Max(_currentHP, 0);
        _healthBar.Value = _currentHP;
        _regenTimer = 0f; // reset timer saat kena hit

        if (_currentHP <= 0)
        {
            Die();
        }
    }

    private void PlayAnim(string name)
    {
        if (_animPlayer.IsPlaying() && _animPlayer.CurrentAnimation == name) return;
        _animPlayer.Play(name);
    }

    public void Die()
    {
        if (_isDead) return;
        Engine.TimeScale = 0.2f; // Slow motion
        Engine.TimeScale = 1.0f;

        _isDead = true;
        _velocity = Vector3.Zero;
        Velocity = Vector3.Zero;

        PlayAnim("die");
        GD.Print("Player is dead.");

        // efek zoom out kamera
        if (_springArm != null)
        {
            var tween = CreateTween();

            // Zoom out (menjauh)
            tween.TweenProperty(_springArm, "spring_length", _springArm.SpringLength + 5.0f, 2.0f)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Cubic);

            // Naikkan kamera (pitch tilt)
            tween.TweenProperty(_springArm, "rotation_degrees:x", _springArm.RotationDegrees.X - 15.0f, 2.0f)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Cubic);
        }

        GD.Print("SpringArm Rotation BEFORE: ", _springArm.RotationDegrees.X);
        GD.Print("SpringArm Rotation AFTER: ", _springArm.RotationDegrees.X);

        _gameOverUI.Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Visible; // biar bisa klik tombol

        GD.Print("Game Over UI path: ", GameOverUIPath);
        GD.Print("GameOver visible: ", _gameOverUI.Visible);


    }

    public void ShowItemPanel(string itemName)
    {
        _itemInfoLabel.Text = $"You collected: {itemName}";
        _itemInfoPanel.Visible = true;

        var tween = CreateTween();
        tween.TweenCallback(Callable.From(() => _itemInfoPanel.Visible = false))
            .SetDelay(3f); // Hilang setelah 3 detik
    }
    
    public void SetNearbyItem(CollectibleItem item)
    {
        _currentItem = item;
    }

}
