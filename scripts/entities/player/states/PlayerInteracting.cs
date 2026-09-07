using Godot;

public partial class PlayerInteracting : State
{
    [Export]
    private CharacterBody2D _body;

    [Export]
    private AnimatedSprite2D _sprite;

    [Export]
    private State _standingState;

    public override void Enter()
    {
        if (_body != null)
        {
            _body.Velocity = Vector2.Zero;
        }

        if (_sprite != null)
        {
            _sprite.Play("Idle");
        }
    }

    public override void UpdatePhysics(double delta)
    {
        if (_body != null)
        {
            _body.Velocity = Vector2.Zero;
        }
    }

    public void CompleteInteraction()
    {
        if (_standingState != null)
        {
            Transition(_standingState);
        }
    }
}
