using Godot;
using Zeldavania.Combat;

public partial class Arrow : Area2D
{
    [Export]
    public float LaunchSpeed { get; set; } = 360f;

    [Export]
    public float StraightDistance { get; set; } = 128f;

    [Export]
    public float GravityAcceleration { get; set; } = 600f;

    [Export]
    public float DespawnDelay { get; set; } = 1.5f;

    [Export]
    public int Damage { get; set; } = 1;

    [Export]
    public float KnockbackForce { get; set; } = 150f;

    [ExportGroup("Water Physics")]
    [Export]
    public float WaterLinearDrag { get; set; } = 2.6f;

    [Export]
    public float WaterDeceleration { get; set; } = 175f;

    [Export]
    public float WaterGravity { get; set; } = 170f;

    [Export]
    public float MinLethalSpeed { get; set; } = 100f;

    [Export]
    public float WaterSpentLingerDuration { get; set; } = 0.35f;

    [Export]
    public float WaterFadeDuration { get; set; } = 0.5f;

    [Export]
    private Sprite2D _sprite;

    [Export]
    private CollisionShape2D _collisionShape;

    [Export]
    private Flammable2D _flammable;

    [Export]
    private WaterDetector2D _waterDetector;

    private Vector2 _velocity = Vector2.Zero;
    private float _distanceTraveled = 0f;
    private bool _isLaunched = false;
    private bool _isEmbedded = false;
    private bool _isInWater = false;
    private bool _isDisappearing = false;

    public bool IsBurning => _flammable != null && _flammable.IsBurning;
    public bool IsInWater => _isInWater;
    public bool IsLethal =>
        !_isDisappearing && !_isEmbedded && _velocity.Length() >= MinLethalSpeed;

    public override void _Ready()
    {
        BodyEntered += HandleBodyEntered;
        AreaEntered += HandleAreaEntered;

        if (_flammable == null)
        {
            _flammable = GetNodeOrNull<Flammable2D>("Flammable2D");
        }

        if (_waterDetector == null)
        {
            _waterDetector = GetNodeOrNull<WaterDetector2D>("WaterDetector2D");
        }

        if (_waterDetector != null)
        {
            _waterDetector.BodyEntered += HandleWaterBodyEntered;
        }
    }

    public override void _ExitTree()
    {
        BodyEntered -= HandleBodyEntered;
        AreaEntered -= HandleAreaEntered;

        if (_waterDetector != null)
        {
            _waterDetector.BodyEntered -= HandleWaterBodyEntered;
        }
    }

    public void Ignite()
    {
        if (_isInWater)
        {
            return;
        }

        if (_flammable == null)
        {
            _flammable = GetNodeOrNull<Flammable2D>("Flammable2D");
        }
        _flammable?.Ignite();
    }

    public void Extinguish()
    {
        _flammable?.Extinguish();
    }

    public void Launch(Vector2 direction, float speed = -1f)
    {
        _isLaunched = true;
        _isEmbedded = false;
        _isInWater = false;
        _isDisappearing = false;
        _distanceTraveled = 0f;

        float launchSpeed = speed > 0 ? speed : LaunchSpeed;
        _velocity = direction.Normalized() * launchSpeed;
        Rotation = _velocity.Angle();

        if (
            _waterDetector != null
            && _waterDetector.GetOverlappingBodies().Count > 0
        )
        {
            EnterWater();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isLaunched || _isEmbedded)
        {
            return;
        }

        float dt = (float)delta;

        if (_isInWater)
        {
            // Apply fluid drag (linear drag + constant deceleration)
            _velocity *= Mathf.Max(0f, 1f - WaterLinearDrag * dt);
            _velocity = _velocity.MoveToward(
                Vector2.Zero,
                WaterDeceleration * dt
            );

            // Apply downward gravity in water so the heavy arrowhead naturally points downward
            _velocity.Y += WaterGravity * dt;

            // Check if speed dropped below lethal/active threshold
            if (!_isDisappearing && _velocity.Length() < MinLethalSpeed)
            {
                StartDisappearing(
                    WaterFadeDuration,
                    delay: WaterSpentLingerDuration
                );
            }
        }
        else
        {
            Vector2 step = _velocity * dt;
            _distanceTraveled += step.Length();

            if (_distanceTraveled >= StraightDistance)
            {
                _velocity.Y += GravityAcceleration * dt;
            }
        }

        if (_velocity.LengthSquared() > 1f)
        {
            Rotation = _velocity.Angle();
        }

        GlobalPosition += _velocity * dt;
    }

    private void HandleWaterBodyEntered(Node2D body)
    {
        if (!_isInWater)
        {
            EnterWater();
        }
    }

    private void HandleBodyEntered(Node2D body)
    {
        if (_isEmbedded)
        {
            return;
        }

        Embed(body);
    }

    private void HandleAreaEntered(Area2D area)
    {
        if (_isEmbedded || !IsLethal)
        {
            return;
        }

        if (area is Hurtbox hurtbox)
        {
            hurtbox.ReceiveHit(new Hit(Damage, GlobalPosition, KnockbackForce));
            QueueFree();
        }
    }

    private void EnterWater()
    {
        _isInWater = true;
        Extinguish();
    }

    private void StartDisappearing(float duration, float delay = 0f)
    {
        if (_isDisappearing)
        {
            return;
        }

        _isDisappearing = true;

        if (_collisionShape != null)
        {
            _collisionShape.SetDeferred(
                CollisionShape2D.PropertyName.Disabled,
                true
            );
        }

        var tween = CreateTween();
        if (delay > 0f)
        {
            tween.TweenInterval(delay);
        }
        if (_sprite != null)
        {
            tween.TweenProperty(_sprite, "modulate:a", 0f, duration);
        }
        tween.TweenCallback(Callable.From(QueueFree));
    }

    private void Embed(Node target = null)
    {
        if (_isEmbedded)
        {
            return;
        }

        _isEmbedded = true;
        _velocity = Vector2.Zero;

        SetDeferred(PropertyName.Monitoring, false);
        SetDeferred(PropertyName.Monitorable, false);

        if (target != null && IsInstanceValid(target) && target.IsInsideTree())
        {
            CallDeferred(nameof(AttachToTarget), target);
        }

        float fadeDuration = 0.3f;
        float delay = Mathf.Max(0f, DespawnDelay - fadeDuration);
        StartDisappearing(fadeDuration, delay);
    }

    private void AttachToTarget(Node target)
    {
        if (IsInstanceValid(target) && target.IsInsideTree() && IsInsideTree())
        {
            Reparent(target, keepGlobalTransform: true);
        }
    }
}
