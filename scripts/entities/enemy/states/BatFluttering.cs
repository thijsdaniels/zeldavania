using Godot;

public partial class BatFluttering : State
{
    [ExportGroup("Dependencies")]
    [Export]
    private Enemy _enemy;

    [Export]
    private AnimatedSprite2D _sprite;

    [ExportGroup("Transitions")]
    [Export]
    private State _onSwoopReady;

    [ExportGroup("Transitions")]
    [Export]
    private State _onTargetLost;

    [ExportGroup("Tuning")]
    [Export]
    private string _animation = "Flying";

    [Export]
    private float _flutterSpeed = 50f;

    [Export]
    private float _waveFrequency = 6f;

    [Export]
    private float _waveAmplitude = 25f;

    [Export]
    private float _swoopCooldown = 2.5f;

    [Export]
    private float _attackDistance = 160f;

    [Export]
    private float _lostDistance = 250f;

    [Export]
    private float _preferredHeightAboveTarget = 40f;

    private float _time;
    private float _cooldownTimer;

    public override void Enter()
    {
        _time = 0f;
        _cooldownTimer = _swoopCooldown;

        if (_sprite != null && !string.IsNullOrEmpty(_animation))
        {
            _sprite.Play(_animation);
        }
    }

    public override void UpdatePhysics(double delta)
    {
        if (_enemy == null)
            return;

        _time += (float)delta;

        if (_enemy.Target == null)
        {
            if (_onTargetLost != null)
            {
                Transition(_onTargetLost);
            }
            return;
        }

        Vector2 targetPos = _enemy.Target.GlobalPosition;
        float distance = _enemy.GlobalPosition.DistanceTo(targetPos);

        if (distance > _lostDistance)
        {
            _enemy.Target = null;
            if (_onTargetLost != null)
            {
                Transition(_onTargetLost);
            }
            return;
        }

        // Navigate horizontally toward target and stay elevated above it
        Vector2 desiredPos = new Vector2(
            targetPos.X,
            targetPos.Y - _preferredHeightAboveTarget
        );

        float dirX = Mathf.Sign(desiredPos.X - _enemy.GlobalPosition.X);
        float targetVelX = dirX * _flutterSpeed;
        float currentVelX = Mathf.MoveToward(
            _enemy.Velocity.X,
            targetVelX,
            120f * (float)delta
        );

        // Sinusoidal vertical bobbing + height correction
        float waveY = Mathf.Sin(_time * _waveFrequency) * _waveAmplitude;
        float heightError = desiredPos.Y - _enemy.GlobalPosition.Y;
        float heightCorrection = Mathf.Clamp(heightError * 1.5f, -40f, 40f);
        float currentVelY = waveY + heightCorrection;

        _enemy.Velocity = new Vector2(currentVelX, currentVelY);

        if (_sprite != null && Mathf.Abs(_enemy.Velocity.X) > 1f)
        {
            _sprite.FlipH = _enemy.Velocity.X < 0;
        }

        // Countdown to next swoop attack
        _cooldownTimer -= (float)delta;
        if (
            _cooldownTimer <= 0f
            && distance <= _attackDistance
            && _enemy.GlobalPosition.Y < targetPos.Y
        )
        {
            if (_onSwoopReady != null)
            {
                Transition(_onSwoopReady);
            }
        }
    }
}
