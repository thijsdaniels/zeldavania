using Godot;
using Zeldavania.Combat;

public partial class EnemyHurt : State
{
    [ExportGroup("Dependencies")]
    [Export]
    private Enemy _enemy;

    [Export]
    private Hurtbox _hurtbox;

    [Export]
    private Damageable _damageable;

    [Export]
    private AnimatedSprite2D _sprite;

    [Export]
    private AudioStreamPlayer2D _soundEffect;

    [ExportGroup("Transitions")]
    [Export]
    private State _chasingState;

    [Export]
    private State _standingState;

    [Export]
    private State _dyingState;

    [ExportGroup("Tuning")]
    [Export]
    private float _hitstunDuration = 0.18f;

    [Export]
    private float _horizontalKnockbackRatio = 0.55f;

    [Export]
    private float _upwardKnockback = 80f;

    [Export]
    private float _gravity = 500f;

    [Export]
    private float _friction = 400f;

    private float _hitstunTimer;
    private Vector2 _lastHitOrigin;
    private float _lastHitForce;
    private Tween _flashTween;

    public override void _Ready()
    {
        if (_hurtbox != null)
        {
            _hurtbox.OnHurt += HandleHurt;
        }

        if (_damageable != null)
        {
            _damageable.OnDepleted += HandleDepleted;
        }
    }

    public override void _ExitTree()
    {
        if (_hurtbox != null)
        {
            _hurtbox.OnHurt -= HandleHurt;
        }

        if (_damageable != null)
        {
            _damageable.OnDepleted -= HandleDepleted;
        }

        _flashTween?.Kill();
    }

    private void HandleHurt(int damage, Vector2 origin, float force)
    {
        _lastHitOrigin = origin;
        _lastHitForce = force;

        if (_damageable != null && _damageable.IsDepleted)
        {
            if (_dyingState != null)
            {
                Transition(_dyingState);
            }
            return;
        }

        Transition(this);
    }

    private void HandleDepleted()
    {
        if (_hurtbox != null)
        {
            _hurtbox.IsInvulnerable = true;
        }

        if (_dyingState != null)
        {
            Transition(_dyingState);
        }
    }

    public override void Enter()
    {
        if (_hurtbox != null)
        {
            _hurtbox.IsInvulnerable = true;
        }

        _soundEffect?.Play();
        _hitstunTimer = _hitstunDuration;

        Flash();

        // Calculate knockback away from hit origin
        float dirX = Mathf.Sign(_enemy.GlobalPosition.X - _lastHitOrigin.X);
        if (dirX == 0)
            dirX = -Mathf.Sign(_sprite != null ? (_sprite.FlipH ? -1f : 1f) : 1f);
        if (dirX == 0)
            dirX = 1f;

        float horizontalSpeed = _lastHitForce * _horizontalKnockbackRatio;
        _enemy.Velocity = new Vector2(
            dirX * horizontalSpeed,
            -_upwardKnockback
        );
    }

    public override void Exit()
    {
        _flashTween?.Kill();
        if (_sprite != null)
        {
            _sprite.Modulate = new Color(1, 1, 1, 1);
            _sprite.SpeedScale = 1.0f;
        }

        if (_hurtbox != null && (_damageable == null || !_damageable.IsDepleted))
        {
            _hurtbox.IsInvulnerable = false;
            _hurtbox.CheckOverlapping();
        }
    }

    public override void UpdatePhysics(double delta)
    {
        _hitstunTimer -= (float)delta;

        // Apply friction and gravity during knockback
        _enemy.Velocity = new Vector2(
            Mathf.MoveToward(_enemy.Velocity.X, 0, _friction * (float)delta),
            _enemy.Velocity.Y + _gravity * (float)delta
        );

        if (_hitstunTimer <= 0)
        {
            if (_enemy.Target != null && _chasingState != null)
            {
                Transition(_chasingState);
            }
            else if (_standingState != null)
            {
                Transition(_standingState);
            }
        }
    }

    private void Flash()
    {
        if (_sprite == null)
            return;

        _flashTween?.Kill();
        _flashTween = CreateTween();
        _flashTween.SetLoops(3);
        _flashTween.TweenProperty(_sprite, "modulate", new Color(1f, 0.2f, 0.2f, 0.4f), 0.035f);
        _flashTween.TweenProperty(_sprite, "modulate", new Color(1f, 1f, 1f, 1.0f), 0.035f);
    }
}
