using Godot;
using Zeldavania.Extensions;

public partial class PlayerRunning : State
{
    [Export]
    private CharacterBody2D _body;

    [Export]
    private Player _player;

    [Export]
    private AnimatedSprite2D _sprite;

    [ExportGroup("Movement")]
    [Export]
    private float _acceleration = 1600;

    [Export]
    private float _deceleration = 800;

    [Export]
    private float _maximumVelocity = 70;

    [ExportGroup("Standing")]
    [Export]
    private State _standingState;

    [ExportGroup("Falling")]
    [Export]
    private State _fallingState;

    [ExportGroup("Jumping")]
    [Export]
    private State _jumpingState;

    [ExportGroup("Climbing")]
    [Export]
    private State _climbingState;

    [Export]
    private TileDetector2D _ladderDetector;

    public override void _Ready()
    {
        if (_player == null && _body is Player player)
        {
            _player = player;
        }
    }

    public override void Enter()
    {
        _sprite.Play("Run");
    }

    public override void UpdatePhysics(double delta)
    {
        float direction = Controller.GetHorizontalDirection();

        switch (true)
        {
            case true when !_body.IsOnFloor():
                if (_fallingState is PlayerFalling fallingState)
                {
                    fallingState.EnableCoyoteTime();
                }
                Transition(_fallingState);
                break;

            case true when this.TryTriggerItemAction(_player, out State targetState):
                Transition(targetState);
                break;

            case true when direction == 0 && _body.Velocity.X == 0:
                Transition(_standingState);
                break;

            case true when Input.IsActionJustPressed(Controller.A):
                Transition(_jumpingState);
                break;

            case true
                when Input.IsActionJustPressed(Controller.Down)
                    && _body.IsOnOneWayPlatform():
                _body.SetCollisionMaskValue(2, false);
                _body.Velocity = new Vector2(_body.Velocity.X, Mathf.Max(_body.Velocity.Y, 50f));
                Transition(_fallingState);
                break;

            case true
                when _ladderDetector.IsOverlapping
                    && Input.IsActionPressed(Controller.Up):
                Transition(_climbingState);
                break;

            default:
                Run(delta, direction);
                break;
        }
    }

    private void Run(double delta, float direction)
    {
        _body.MoveWithInertia(
            direction: direction,
            acceleration: _acceleration * (float)delta,
            deceleration: _deceleration * (float)delta,
            limit: _maximumVelocity
        );

        _sprite.SynchronizeAnimation(-direction);
    }
}
