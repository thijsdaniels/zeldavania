using System;
using System.Collections.Generic;
using Godot;

namespace Zeldavania.UserInterface;

[GlobalClass]
public partial class DialogueBox : Control
{
    [Signal]
    public delegate void DialogueOpenedEventHandler();

    [Signal]
    public delegate void DialogueClosedEventHandler();

    [Export]
    private RichTextLabel _messageLabel;

    [Export]
    private Control _container;

    [Export]
    private Control _continuePrompt;

    [Export]
    private Label _actionLabel;

    [Export]
    private AudioStreamPlayer _textAudio;

    [Export]
    private float _charactersPerSecond = 35f;

    [Export]
    private int _maxLinesPerPage = 3;

    public bool IsOpen { get; private set; }

    private readonly List<string> _pages = new();
    private int _currentPageIndex = 0;
    private bool _isRevealing;
    private string _currentPageText = "";
    private double _visibleCountAccumulator = 0;
    private Action _onClosedCallback;
    private int _lastSoundChar = 0;

    public override void _Ready()
    {
        Visible = false;
        IsOpen = false;
        if (_continuePrompt != null)
        {
            _continuePrompt.Visible = false;
        }
    }

    public void Open(string message, Action onClosed = null)
    {
        _onClosedCallback = onClosed;
        _pages.Clear();
        _currentPageIndex = 0;

        BuildPages(message ?? "");

        if (_pages.Count == 0)
        {
            _pages.Add("");
        }

        IsOpen = true;
        Visible = true;
        EmitSignal(SignalName.DialogueOpened);

        DisplayPage(0);
    }

    private void BuildPages(string fullMessage)
    {
        string normalized = fullMessage.Replace("\r\n", "\n").Replace("\r", "\n");
        string[] explicitSections = normalized.Split(new[] { "\n---\n", "\f" }, StringSplitOptions.None);

        foreach (var section in explicitSections)
        {
            string[] rawLines = section.Split('\n');
            var pageLines = new List<string>();

            for (int i = 0; i < rawLines.Length; i++)
            {
                pageLines.Add(rawLines[i]);

                if (pageLines.Count == _maxLinesPerPage || i == rawLines.Length - 1)
                {
                    _pages.Add(string.Join("\n", pageLines));
                    pageLines.Clear();
                }
            }
        }
    }

    private void DisplayPage(int index)
    {
        _currentPageIndex = index;
        _currentPageText = _pages[index];
        _visibleCountAccumulator = 0;
        _lastSoundChar = 0;
        _isRevealing = true;

        if (_messageLabel != null)
        {
            _messageLabel.Text = _currentPageText;
            _messageLabel.VisibleCharacters = 0;
        }

        if (_continuePrompt != null)
        {
            _continuePrompt.Visible = false;
        }

        UpdatePromptLabel();
    }

    private void UpdatePromptLabel()
    {
        if (_actionLabel != null)
        {
            bool isLastPage = _currentPageIndex >= _pages.Count - 1;
            _actionLabel.Text = isLastPage ? "Close" : "Continue";
        }
    }

    public void Close()
    {
        if (!IsOpen)
        {
            return;
        }

        IsOpen = false;
        _isRevealing = false;
        Visible = false;

        var cb = _onClosedCallback;
        _onClosedCallback = null;
        cb?.Invoke();

        EmitSignal(SignalName.DialogueClosed);
    }

    public override void _Process(double delta)
    {
        if (!IsOpen)
        {
            return;
        }

        if (_isRevealing)
        {
            _visibleCountAccumulator += delta * _charactersPerSecond;
            int visibleChars = (int)_visibleCountAccumulator;

            if (_messageLabel != null)
            {
                _messageLabel.VisibleCharacters = visibleChars;
            }

            if (visibleChars > _lastSoundChar)
            {
                _lastSoundChar = visibleChars;
                if (_textAudio != null && !_textAudio.Playing)
                {
                    _textAudio.Play();
                }
            }

            if (visibleChars >= _currentPageText.Length)
            {
                CompleteReveal();
            }
            else if (Input.IsActionJustPressed(Controller.A))
            {
                CompleteReveal();
                GetViewport().SetInputAsHandled();
            }
        }
        else
        {
            if (Input.IsActionJustPressed(Controller.A))
            {
                GetViewport().SetInputAsHandled();
                if (_currentPageIndex < _pages.Count - 1)
                {
                    DisplayPage(_currentPageIndex + 1);
                }
                else
                {
                    Close();
                }
            }
        }
    }

    private void CompleteReveal()
    {
        _isRevealing = false;
        if (_messageLabel != null)
        {
            _messageLabel.VisibleCharacters = -1;
        }
        UpdatePromptLabel();
        if (_continuePrompt != null)
        {
            _continuePrompt.Visible = true;
        }
    }
}
