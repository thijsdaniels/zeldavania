using Godot;

public partial class PauseMenu : Control
{
    [Export]
    private Button _resumeButton;

    [Export]
    private Button _restartButton;

    [Export]
    private Button _quitButton;

    [Export]
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        Visible = false;

        _resumeButton.Pressed += OnResumePressed;
        _restartButton.Pressed += OnRestartPressed;
        _quitButton.Pressed += OnQuitPressed;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Start"))
        {
            if (!GetTree().Paused)
            {
                Pause();
                GetViewport().SetInputAsHandled();
            }
            else if (Visible)
            {
                Resume();
                GetViewport().SetInputAsHandled();
            }
        }
    }

    public void OnResumePressed()
    {
        Resume();
    }

    public void OnRestartPressed()
    {
        Restart();
    }

    public void OnQuitPressed()
    {
        Quit();
    }

    private void Pause()
    {
        GetTree().Paused = true;

        Visible = true;
        _animationPlayer?.Play("Open");
        _resumeButton?.GrabFocus();
    }

    private void Resume()
    {
        GetTree().Paused = false;

        Visible = false;
        _animationPlayer?.PlayBackwards("Open");
    }

    private void Restart()
    {
        GetTree().Paused = false;
        GetTree().ReloadCurrentScene();
    }

    private void Quit()
    {
        GetTree().Quit();
    }
}
