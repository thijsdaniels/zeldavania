using Godot;

public partial class PlayerClimbing : State
{
    [Export]
    private CharacterBody2D _body;

    [Export]
    private AnimatedSprite2D _sprite;

    [ExportGroup("Climbing")]
    [Export]
    private TileDetector2D _ladderDetector;

    [Export]
    private Vector2 _climbVelocity = new(30, 40);

    [Export]
    private Vector2 _climbAcceleration = new(300, 400);

    [Export]
    private float _leapVelocity = 40;

    [Export]
    private float _centeringSpeed = 30f;

    [Export]
    private Vector2 _climbDeceleration = new(600, 800);

    [ExportGroup("Transitions")]
    [Export]
    private State _fallingState;

    [Export]
    private State _jumpingState;

    [Export]
    private State _standingState;

    public override void Enter()
    {
        _sprite.Play("Climb");
        _ladderDetector.OnTileExited += OnLadderExited;
    }

    public override void Exit()
    {
        _ladderDetector.OnTileExited -= OnLadderExited;
    }

    private void OnLadderExited()
    {
        if (Input.IsActionPressed(Controller.Up))
        {
            _body.Velocity = new Vector2(
                _body.Velocity.X,
                -_leapVelocity
            );
        }

        Transition(_fallingState);
    }

    public override void UpdatePhysics(double delta)
    {
        switch (true)
        {
            case true
                when _body.IsOnFloor()
                    && Input.IsActionPressed(Controller.Down):
                Transition(_standingState);
                return;

            case true when Input.IsActionJustPressed(Controller.A):
                Transition(_jumpingState);
                return;

            case true when Input.IsActionJustPressed(Controller.B):
                Transition(_fallingState);
                return;
        }

        Climb(delta);
    }

    private void Climb(double delta)
    {
        Vector2 direction = Controller.GetDirection();

        if (direction.X == 0 && direction.Y != 0)
        {
            float targetX = Mathf.Floor(_body.GlobalPosition.X / 16f) * 16f + 8f;
            float newX = Mathf.MoveToward(_body.GlobalPosition.X, targetX, _centeringSpeed * (float)delta);
            _body.GlobalPosition = new Vector2(newX, _body.GlobalPosition.Y);
        }

        _body.MoveWithInertia(
            direction: direction,
            acceleration: _climbAcceleration * (float)delta,
            deceleration: _climbDeceleration * (float)delta,
            limit: _climbVelocity
        );

        _sprite.SynchronizeAnimation(-direction);
    }
}
