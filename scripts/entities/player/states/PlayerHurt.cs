using Godot;
using Zeldavania.Combat;

public partial class PlayerHurt : State
{
    [ExportGroup("Dependencies")]
    [Export]
    private CharacterBody2D _body;

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
    private State _standingState;

    [Export]
    private State _fallingState;

    [ExportGroup("Tuning")]
    [Export]
    private float _hitstunDuration = 0.25f;

    [Export]
    private float _invulnerabilityDuration = 0.75f;

    [Export]
    private float _upwardKnockbackRatio = 0.5f;

    [Export]
    private float _gravity = 500f;

    [Export]
    private float _friction = 300f;

    private float _hitstunTimer;
    private Vector2 _lastHitOrigin;
    private float _lastHitForce;
    private Tween _blinkTween;

    public override void _Ready()
    {
        if (_body == null)
            GD.PushError($"[PlayerHurt] '{GetPath()}' has no CharacterBody2D assigned!");
        if (_hurtbox == null)
            GD.PushError($"[PlayerHurt] '{GetPath()}' has no Hurtbox assigned!");
        if (_damageable == null)
            GD.PushError($"[PlayerHurt] '{GetPath()}' has no Damageable assigned!");
        if (_sprite == null)
            GD.PushError($"[PlayerHurt] '{GetPath()}' has no AnimatedSprite2D assigned!");

        if (_hurtbox != null)
        {
            _hurtbox.OnHurt += HandleHurt;
        }
    }

    public override void _ExitTree()
    {
        if (_hurtbox != null)
        {
            _hurtbox.OnHurt -= HandleHurt;
        }
        _blinkTween?.Kill();
    }

    private void HandleHurt(int damage, Vector2 origin, float force)
    {
        _lastHitOrigin = origin;
        _lastHitForce = force;
        Transition(this);
    }

    public override void Enter()
    {
        _soundEffect?.Play();
        _sprite?.Play("Hurt");
        _hitstunTimer = _hitstunDuration;

        StartInvulnerability();

        float dirX = Mathf.Sign(_body.GlobalPosition.X - _lastHitOrigin.X);
        if (dirX == 0)
        {
            dirX = -Mathf.Sign(_sprite.Scale.X);
            if (dirX == 0)
                dirX = 1f;
        }

        _body.Velocity = new Vector2(
            dirX * _lastHitForce,
            -_lastHitForce * _upwardKnockbackRatio
        );
    }

    public override void UpdatePhysics(double delta)
    {
        _hitstunTimer -= (float)delta;

        // Apply gravity and deceleration during hitstun knockback
        _body.Velocity = new Vector2(
            Mathf.MoveToward(_body.Velocity.X, 0, _friction * (float)delta),
            _body.Velocity.Y + _gravity * (float)delta
        );

        if (_hitstunTimer <= 0)
        {
            if (_body.IsOnFloor())
            {
                Transition(_standingState);
            }
            else
            {
                Transition(_fallingState);
            }
        }
    }

    private void StartInvulnerability()
    {
        if (_hurtbox != null)
        {
            _hurtbox.IsInvulnerable = true;
        }

        _blinkTween?.Kill();
        _blinkTween = CreateTween();
        int loops = Mathf.Max(1, (int)(_invulnerabilityDuration / 0.1f));
        _blinkTween.SetLoops(loops);
        _blinkTween.TweenProperty(_sprite, "modulate:a", 0.3f, 0.05f);
        _blinkTween.TweenProperty(_sprite, "modulate:a", 1.0f, 0.05f);
        _blinkTween.Finished += () =>
        {
            if (_hurtbox != null)
            {
                _hurtbox.IsInvulnerable = false;
                _hurtbox.CheckOverlapping();
            }
            if (_sprite != null)
            {
                _sprite.Modulate = new Color(1, 1, 1, 1);
            }
        };
    }
}
