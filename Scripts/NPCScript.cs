using Godot;
using System;

public partial class NPCScript : Area3D
{
    [Export] public string NPCName = "Tetua Desa - Arka";
    [Export] public NodePath DialogUIPath;
    [Export] public float KetikDelay = 0.03f;
    [Export(PropertyHint.MultilineText)] public string[] DialogLines = new string[]
    {
        "[Tetua Desa] Selamat datang, Arka. Sudah lama kami menunggumu...",
        "[Arka] Terima kasih, Pak. Apa benar ada sesuatu yang harus saya cari di sini?",
        "[Tetua Desa] Ya, kami percaya hanya kaulah yang bisa menjaga warisan leluhur ini.",
        "[Arka] Warisan? Maksudnya... seperti artefak kuno?",
        "[Tetua Desa] Benar. Di desa ini tersembunyi benda-benda sakral, termasuk gamelan Saron dan wayang kuno.",
        "[Arka] Luar biasa... Saya akan mencarinya.",
        "[Tetua Desa] Carilah mereka, dan jangan ragu bertanya pada penjaga candi jika kau merasa tersesat.",
        "[Arka] Baik, saya mengerti. Terima kasih atas arahannya.",
        "[Tetua Desa] Semoga para leluhur selalu membimbing langkahmu, Arka..."
    };


    private Control _dialogUI;
    private Label _npcNameLabel;
    private RichTextLabel _dialogText;
    private Button _nextButton;

    private int _dialogIndex = 0;
    private string _currentLine = "";
    private int _charIndex = 0;
    private bool _playerInArea = false;
    private bool _isTyping = false;

    private Timer _typeTimer;

    public override void _Ready()
    {
        _dialogUI = GetNode<Control>(DialogUIPath);
        _npcNameLabel = _dialogUI.GetNode<Label>("Panel/npcNameLabel");
        _dialogText = _dialogUI.GetNode<RichTextLabel>("Panel/dialogText");
        _nextButton = _dialogUI.GetNode<Button>("Panel/NextButton");

        _nextButton.Visible = false;
        _nextButton.Pressed += OnNextPressed;
        _dialogUI.Visible = false;

        _typeTimer = new Timer();
        _typeTimer.WaitTime = KetikDelay;
        _typeTimer.OneShot = false;
        _typeTimer.Timeout += OnTypeTimerTick;
        AddChild(_typeTimer);

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public override void _Process(double delta)
    {
        if (_playerInArea && Input.IsActionJustPressed("interact") && !_dialogUI.Visible)
        {
            StartDialog();
        }

        // Tekan Enter sebagai alternatif tombol Next
        if (_dialogUI.Visible && Input.IsActionJustPressed("continue_dialog") && !_isTyping)
        {
            OnNextPressed();
        }
    }

    private void OnBodyEntered(Node body)
    {
        if (body is CharacterBody3D)
            _playerInArea = true;
    }

    private void OnBodyExited(Node body)
    {
        if (body is CharacterBody3D)
        {
            _playerInArea = false;
            ResetDialog();
        }
    }

    private void StartDialog()
    {
        _dialogUI.Visible = true;
        _npcNameLabel.Text = NPCName;
        _dialogIndex = 0;
        ShowLineWithTyping();
    }

    private void ShowLineWithTyping()
    {
        if (_dialogIndex >= DialogLines.Length)
        {
            ResetDialog();
            return;
        }

        _currentLine = DialogLines[_dialogIndex];
        _dialogText.Text = "";
        _charIndex = 0;
        _isTyping = true;
        _nextButton.Visible = false;
        _typeTimer.Start();
    }

    private void OnTypeTimerTick()
    {
        if (_charIndex < _currentLine.Length)
        {
            _dialogText.Text += _currentLine[_charIndex];
            _charIndex++;
        }
        else
        {
            _typeTimer.Stop();
            _isTyping = false;
            _nextButton.Visible = true;
        }
    }

    private void OnNextPressed()
    {
        if (_isTyping)
        {
            // Skip animasi ketik, tampilkan semua huruf langsung
            _dialogText.Text = _currentLine;
            _typeTimer.Stop();
            _isTyping = false;
            _nextButton.Visible = true;
        }
        else
        {
            _dialogIndex++;
            ShowLineWithTyping();
        }
    }

    private void ResetDialog()
    {
        _dialogUI.Visible = false;
        _dialogIndex = 0;
        _charIndex = 0;
        _typeTimer.Stop();
    }
}
