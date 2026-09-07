using Godot;
using Zeldavania.Extensions;

public partial class PlayerStanding : State
{
    [Export]
    private CharacterBody2D _body;

    [Export]
    private Player _player;

    [Export]
    private AnimatedSprite2D _sprite;

    [ExportGroup("Climbing")]
    [Export]
    private State _climbingState;

    [Export]
    private TileDetector2D _ladderDetector;

    [ExportGroup("Falling")]
    [Export]
    private State _fallingState;

    [ExportGroup("Running")]
    [Export]
    private State _runningState;

    [ExportGroup("Jumping")]
    [Export]
    private State _jumpingState;

    public override void _Ready()
    {
        if (_player == null && _body is Player player)
        {
            _player = player;
        }
    }

    public override void Enter()
    {
        _sprite.Play("Idle");
    }

    public override void UpdatePhysics(double delta)
    {
        switch (true)
        {
            case true when !_body.IsOnFloor():
                Transition(_fallingState);
                break;

            case true when this.TryTriggerItemAction(_player, out State targetState):
                Transition(targetState);
                break;

            case true when Controller.GetHorizontalDirection() != 0:
                Transition(_runningState);
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
        }
    }
}
