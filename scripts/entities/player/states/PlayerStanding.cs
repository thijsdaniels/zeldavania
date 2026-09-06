using Godot;

public partial class PlayerStanding : State
{
    [Export]
    private CharacterBody2D _body;

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

    [ExportGroup("Attacking")]
    [Export]
    private State _attackingState;

    [ExportGroup("Aiming")]
    [Export]
    private State _aimingState;

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

            case true when Input.IsActionJustPressed(Controller.X):
                Transition(_attackingState);
                break;

            case true when Input.IsActionJustPressed(Controller.B):
                Transition(_aimingState);
                break;

            case true when Controller.GetHorizontalDirection() != 0:
                Transition(_runningState);
                break;

            // case true
            //     when interactionDetector.IsOverlapping
            //         && Input.IsActionJustPressed(Controller.A):
            //     Transition(_interactingState);
            //     break;

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
