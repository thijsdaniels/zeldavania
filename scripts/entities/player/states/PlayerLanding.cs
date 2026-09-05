using Godot;

public partial class PlayerLanding : State
{
    [Export]
    private CharacterBody2D _body;

    [ExportGroup("Landing")]
    [Export]
    private AudioStreamPlayer2D _soundEffect;

    [ExportGroup("Standing")]
    [Export]
    private State _standingState;

    [ExportGroup("Running")]
    [Export]
    private State _runningState;

    public override void Enter()
    {
        _soundEffect.Play();

        switch (true)
        {
            case true when _body.Velocity.X != 0:
                Transition(_runningState);
                break;

            default:
                Transition(_standingState);
                break;
        }
    }
}
