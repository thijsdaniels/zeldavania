using Godot;
using Zeldavania.Combat;

namespace Zeldavania.Objects;

[GlobalClass]
public partial class Jar : Node2D
{
    [Export]
    private Damageable _damageable;

    [Export]
    private Hurtbox _hurtbox;

    [Export]
    private Lootable2D _lootable;

    [Export]
    private PackedScene _destructionEffectScene;

    private bool _isDestroyed;

    public override void _Ready()
    {
        _damageable ??= GetNodeOrNull<Damageable>("Damageable");
        _hurtbox ??= GetNodeOrNull<Hurtbox>("Hurtbox");
        _lootable ??= GetNodeOrNull<Lootable2D>("Lootable2D");

        if (_damageable != null)
        {
            _damageable.OnDepleted += HandleDepleted;
        }
    }

    public override void _ExitTree()
    {
        if (_damageable != null)
        {
            _damageable.OnDepleted -= HandleDepleted;
        }
    }

    private void HandleDepleted()
    {
        if (_isDestroyed)
        {
            return;
        }

        _isDestroyed = true;

        if (_hurtbox != null)
        {
            _hurtbox.SetDeferred(Area2D.PropertyName.Monitoring, false);
            _hurtbox.SetDeferred(Area2D.PropertyName.Monitorable, false);
        }

        if (_destructionEffectScene != null)
        {
            Node effectInstance = _destructionEffectScene.Instantiate();
            if (effectInstance is Node2D effect2D)
            {
                Vector2 centerPosition = _hurtbox != null ? _hurtbox.GlobalPosition : (GlobalPosition + new Vector2(0, -6));
                effect2D.GlobalPosition = centerPosition;
                GetTree()?.CurrentScene?.AddChild(effect2D);
            }
        }

        QueueFree();
    }
}
