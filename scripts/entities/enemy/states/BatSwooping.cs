using Godot;

public partial class BatSwooping : State
{
    [ExportGroup("Dependencies")]
    [Export]
    private Enemy _enemy;

    [Export]
    private AnimatedSprite2D _sprite;

    [ExportGroup("Transitions")]
    [Export]
    private State _onSwoopComplete;

    [ExportGroup("Tuning")]
    [Export]
    private string _animation = "Flying";

    [Export]
    private float _swoopSpeed = 150f;

    [Export]
    private float _overshootDistance = 32f;

    [Export]
    private float _recoveryHeight = 28f;

    private Vector2 _startPos;
    private Vector2 _controlPos;
    private Vector2 _endPos;
    private float _duration;
    private float _progress;

    public override void Enter()
    {
        if (_sprite != null && !string.IsNullOrEmpty(_animation))
        {
            _sprite.Play(_animation);
        }

        if (_enemy == null || _enemy.Target == null)
        {
            if (_onSwoopComplete != null)
            {
                Transition(_onSwoopComplete);
            }
            return;
        }

        _startPos = _enemy.GlobalPosition;
        Vector2 targetPos = _enemy.Target.GlobalPosition;

        float dirX = Mathf.Sign(targetPos.X - _startPos.X);
        if (dirX == 0)
        {
            dirX = (_sprite != null && _sprite.FlipH) ? -1f : 1f;
        }

        // Control point dips below the target to create a deep swoop
        float lowestY = Mathf.Max(_startPos.Y, targetPos.Y) + 8f;
        _controlPos = new Vector2(
            (_startPos.X + targetPos.X) * 0.5f,
            lowestY
        );

        // End point recovers upward past the player
        _endPos = new Vector2(
            targetPos.X + dirX * _overshootDistance,
            targetPos.Y - _recoveryHeight
        );

        float approxLength =
            _startPos.DistanceTo(_controlPos) + _controlPos.DistanceTo(_endPos);
        _duration = Mathf.Max(0.4f, approxLength / _swoopSpeed);
        _progress = 0f;
    }

    public override void UpdatePhysics(double delta)
    {
        if (_enemy == null)
            return;

        _progress += (float)delta / _duration;

        if (_progress >= 1.0f)
        {
            _enemy.Velocity = Vector2.Zero;
            if (_onSwoopComplete != null)
            {
                Transition(_onSwoopComplete);
            }
            return;
        }

        float t = Mathf.Clamp(_progress, 0f, 1f);
        float u = 1f - t;

        // Quadratic Bezier interpolation
        Vector2 targetPosition =
            (u * u * _startPos)
            + (2f * u * t * _controlPos)
            + (t * t * _endPos);

        Vector2 requiredVelocity =
            (targetPosition - _enemy.GlobalPosition) / (float)delta;
        _enemy.Velocity = requiredVelocity;

        if (_sprite != null && Mathf.Abs(_enemy.Velocity.X) > 1f)
        {
            _sprite.FlipH = _enemy.Velocity.X < 0;
        }
    }
}
