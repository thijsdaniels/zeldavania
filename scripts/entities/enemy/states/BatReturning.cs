using Godot;

public partial class BatReturning : State
{
    [ExportGroup("Dependencies")]
    [Export]
    private Enemy _enemy;

    [Export]
    private AnimatedSprite2D _sprite;

    [Export]
    private BatRoosting _roostingState;

    [ExportGroup("Transitions")]
    [Export]
    private State _onRoostReached;

    [ExportGroup("Tuning")]
    [Export]
    private string _animation = "Flying";

    [Export]
    private float _returnSpeed = 60f;

    [Export]
    private float _reachThreshold = 4f;

    public override void Enter()
    {
        if (_enemy != null)
        {
            _enemy.Target = null;
        }

        if (_sprite != null && !string.IsNullOrEmpty(_animation))
        {
            _sprite.Play(_animation);
        }
    }

    public override void UpdatePhysics(double delta)
    {
        if (_enemy == null || _roostingState == null)
        {
            if (_onRoostReached != null)
            {
                Transition(_onRoostReached);
            }
            return;
        }

        Vector2 roostTarget = _roostingState.RoostPosition;
        float distance = _enemy.GlobalPosition.DistanceTo(roostTarget);

        if (distance <= _reachThreshold)
        {
            _enemy.GlobalPosition = roostTarget;
            _enemy.Velocity = Vector2.Zero;

            if (_onRoostReached != null)
            {
                Transition(_onRoostReached);
            }
            return;
        }

        Vector2 direction = (roostTarget - _enemy.GlobalPosition).Normalized();
        _enemy.Velocity = direction * _returnSpeed;

        if (_sprite != null && Mathf.Abs(_enemy.Velocity.X) > 1f)
        {
            _sprite.FlipH = _enemy.Velocity.X < 0;
        }
    }
}
