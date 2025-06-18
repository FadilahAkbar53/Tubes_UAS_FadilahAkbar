using Godot;
using System;

public partial class Premis : Control
{
    [Export] public float KetikDelay = 0.03f;

    private RichTextLabel _label;
    private Timer _ketikTimer;
    private Button _nextButton;
    private Button _skipButton;

    private string _fullText;
    private int _currentCharIndex = 0;
    private bool _isTyping = true;
    private AnimationPlayer _animPlayer;

    public override void _Ready()
    {
        _label = GetNode<RichTextLabel>("RichTextLabel");
        _ketikTimer = GetNode<Timer>("KetikTimer");
        _nextButton = GetNode<Button>("Button");
        _skipButton = GetNode<Button>("SkipButton");
        _animPlayer = GetNode<AnimationPlayer>("Transisi");

        _nextButton.Visible = false;
        _skipButton.Visible = true;

        _nextButton.Pressed += OnNextPressed;
        _skipButton.Pressed += OnSkipPressed;

        _ketikTimer.WaitTime = KetikDelay;
        _ketikTimer.Timeout += OnKetikTimerTick;

        // Isi cerita panjang
        _fullText =
@"Indonesia, tanah yang kaya akan warisan budaya dan sejarah kuno yang terlupakan oleh waktu.

Arka, seorang arkeolog muda, tumbuh dengan cerita rakyat dan dongeng pusaka dari kakeknya, yang mempercayai bahwa peninggalan leluhur masih tersembunyi di pelataran hutan dan reruntuhan candi.

Suatu hari, ia menerima surat misterius berisi peta dan simbol budaya yang menuntunnya ke kampung terpencil, awal dari petualangannya.

Inilah awal dari perjalanan Arka...";

        _label.Text = "";
        _ketikTimer.Start();
    }

    private void OnKetikTimerTick()
    {
        if (_currentCharIndex < _fullText.Length)
        {
            _label.Text += _fullText[_currentCharIndex];
            _currentCharIndex++;
        }
        else
        {
            _ketikTimer.Stop();
            _isTyping = false;
            _nextButton.Visible = true;
            _skipButton.Visible = false;
        }
    }

    private void OnSkipPressed()
    {
        if (_isTyping)
        {
            _label.Text = _fullText;
            _ketikTimer.Stop();
            _isTyping = false;
            _nextButton.Visible = true;
            _skipButton.Visible = false;
        }
    }

    private void OnNextPressed()
    {
        _nextButton.Disabled = true;
        _skipButton.Visible = false;

        if (_animPlayer != null)
        {
            _animPlayer.Play("fade_out"); // pastikan nama animasi fade benar
        }
        else
        {
            GD.PushWarning("⚠️ AnimationPlayer tidak ditemukan, langsung ganti scene");
            ChangeToNextScene(); // fallback
        }
    }

        // Fungsi dipanggil oleh AnimationPlayer di akhir animasi fade
    public void ChangeToNextScene()
    {
        GD.Print("✔️ Scene berpindah...");
        GetTree().ChangeSceneToFile("res://Scene/Demo3.tscn");
    }
}
