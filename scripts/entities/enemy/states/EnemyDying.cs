using Godot;
using Zeldavania.Combat;

public partial class EnemyDying : State
{
    [ExportGroup("Dependencies")]
    [Export]
    public Enemy _enemy;

    [Export]
    public AnimatedSprite2D _sprite;

    [Export]
    public Hurtbox _hurtbox;

    [Export]
    public Area2D _contactHitbox;

    [Export]
    public PackedScene _deathEffectScene = GD.Load<PackedScene>("res://scenes/objects/EnemyDeathEffect.tscn");

    [ExportGroup("Tuning")]
    [Export]
    public string _animation = "Dying";

    [Export]
    public float _friction = 300f;

    [Export]
    public float _gravity = 500f;

    public override void Enter()
    {
        if (_hurtbox != null)
        {
            _hurtbox.IsInvulnerable = true;
            _hurtbox.SetDeferred(Area2D.PropertyName.Monitoring, false);
            _hurtbox.SetDeferred(Area2D.PropertyName.Monitorable, false);
        }

        if (_contactHitbox != null)
        {
            _contactHitbox.SetDeferred(Area2D.PropertyName.Monitoring, false);
            _contactHitbox.SetDeferred(Area2D.PropertyName.Monitorable, false);
        }

        bool hasDyingAnim = _sprite != null
            && _sprite.SpriteFrames != null
            && !string.IsNullOrEmpty(_animation)
            && _sprite.SpriteFrames.HasAnimation(_animation);

        if (hasDyingAnim)
        {
            _sprite.AnimationFinished += OnAnimationFinished;
            _sprite.Play(_animation);
        }
        else
        {
            SpawnDeathEffect();
            _enemy?.QueueFree();
        }
    }

    public override void UpdatePhysics(double delta)
    {
        if (_enemy == null)
            return;

        _enemy.Velocity = new Vector2(
            Mathf.MoveToward(_enemy.Velocity.X, 0, _friction * (float)delta),
            _enemy.Velocity.Y + _gravity * (float)delta
        );
    }

    public void OnAnimationFinished()
    {
        if (_sprite != null)
        {
            _sprite.AnimationFinished -= OnAnimationFinished;
        }

        SpawnDeathEffect();
        _enemy?.QueueFree();
    }

    private void SpawnDeathEffect()
    {
        if (_deathEffectScene != null && _enemy != null)
        {
            var effect = _deathEffectScene.Instantiate<Node2D>();
            effect.GlobalPosition = _enemy.GlobalPosition;
            _enemy.GetParent()?.AddChild(effect);
        }
    }
}
