using Godot;

public partial class PlayerJumping : State
{
    [Export]
    private CharacterBody2D _body;

    [ExportGroup("Jumping")]
    [Export]
    private AudioStreamPlayer2D _soundEffect;

    [Export]
    private float _velocity = 180;

    [ExportGroup("Falling")]
    [Export]
    private State _fallingState;

    public override void Enter()
    {
        Jump();

        if (_fallingState is PlayerFalling fallingState)
        {
            fallingState.NotifyJumpAscent();
        }

        Transition(_fallingState);
    }

    /// @todo Instead of applying a single moment of force, apply force over a
    /// period of time, following a particular curve, until the jump key is
    /// released. At that point, transition to the falling state.
    private void Jump()
    {
        _soundEffect.Play();

        _body.Velocity = new Vector2(_body.Velocity.X, -_velocity);
    }
}
