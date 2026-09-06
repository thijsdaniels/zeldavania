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

    [Export]
    private Sprite2D _sprite;

    [Export]
    private CollisionShape2D _collisionShape;

    private Vector2 _velocity = Vector2.Zero;
    private float _distanceTraveled = 0f;
    private bool _isLaunched = false;
    private bool _isEmbedded = false;

    public override void _Ready()
    {
        BodyEntered += HandleBodyEntered;
        AreaEntered += HandleAreaEntered;
    }

    public override void _ExitTree()
    {
        BodyEntered -= HandleBodyEntered;
        AreaEntered -= HandleAreaEntered;
    }

    public void Launch(Vector2 direction, float speed = -1f)
    {
        _isLaunched = true;
        _isEmbedded = false;
        _distanceTraveled = 0f;

        float launchSpeed = speed > 0 ? speed : LaunchSpeed;
        _velocity = direction.Normalized() * launchSpeed;
        Rotation = _velocity.Angle();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isLaunched || _isEmbedded)
            return;

        Vector2 step = _velocity * (float)delta;
        _distanceTraveled += step.Length();

        if (_distanceTraveled >= StraightDistance)
        {
            _velocity.Y += GravityAcceleration * (float)delta;
            Rotation = _velocity.Angle();
        }

        Position += _velocity * (float)delta;
    }

    private void HandleBodyEntered(Node2D body)
    {
        if (_isEmbedded)
            return;

        Embed(body);
    }

    private void HandleAreaEntered(Area2D area)
    {
        if (_isEmbedded)
            return;

        if (area is Hurtbox hurtbox)
        {
            hurtbox.ReceiveHit(new Hit(Damage, GlobalPosition, KnockbackForce));
            QueueFree();
        }
    }

    private void Embed(Node target = null)
    {
        if (_isEmbedded)
            return;

        _isEmbedded = true;
        _velocity = Vector2.Zero;

        if (_collisionShape != null)
        {
            _collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        }

        SetDeferred(PropertyName.Monitoring, false);
        SetDeferred(PropertyName.Monitorable, false);

        if (target != null && IsInstanceValid(target) && target.IsInsideTree())
        {
            CallDeferred(nameof(AttachToTarget), target);
        }

        float fadeDuration = 0.3f;
        float delay = Mathf.Max(0f, DespawnDelay - fadeDuration);

        var tween = CreateTween();
        tween.TweenInterval(delay);
        if (_sprite != null)
        {
            tween.TweenProperty(_sprite, "modulate:a", 0f, fadeDuration);
        }
        tween.TweenCallback(Callable.From(QueueFree));
    }

    private void AttachToTarget(Node target)
    {
        if (IsInstanceValid(target) && target.IsInsideTree() && IsInsideTree())
        {
            Reparent(target, keepGlobalTransform: true);
        }
    }
}
