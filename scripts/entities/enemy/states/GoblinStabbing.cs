using Godot;
using Zeldavania.Combat;

public partial class GoblinStabbing : State
{
    [Export]
    public Enemy _enemy;

    [Export]
    public AnimatedSprite2D _sprite;

    [Export]
    public Hitbox _hitbox;

    [Export]
    public string _animation = "Stabbing";

    [Export]
    public int _activeFrame = 1;

    [Export]
    public int _damage = 2;

    [Export]
    public float _deceleration = 300f;

    [Export]
    public State _onComplete;

    [Export]
    public State _onFall;

    [Export]
    public Vector2 _hitboxOffset = new Vector2(7, -2.5f);

    public override void Enter()
    {
        if (_enemy != null)
        {
            _enemy.Velocity = new Vector2(0, _enemy.Velocity.Y);
        }

        if (_enemy.Target != null)
        {
            float diffX = _enemy.Target.GlobalPosition.X - _enemy.GlobalPosition.X;
            if (diffX != 0)
            {
                _sprite.FlipH = diffX < 0;
            }
        }

        if (_hitbox != null)
        {
            _hitbox.Damage = _damage;
            _hitbox.Position = new Vector2(
                _hitboxOffset.X * (_sprite.FlipH ? -1 : 1),
                _hitboxOffset.Y
            );
            _hitbox.SetDeferred(Area2D.PropertyName.Monitoring, false);
            _hitbox.SetDeferred(Area2D.PropertyName.Monitorable, false);
        }

        _sprite.SpeedScale = 1.0f;
        _sprite.SetFrameAndProgress(0, 0f);
        _sprite.FrameChanged += OnFrameChanged;
        _sprite.AnimationFinished += OnAnimationFinished;
        _sprite.Play(_animation);

        UpdateHitboxForFrame(_sprite.Frame);
    }

    public override void Exit()
    {
        if (_sprite != null)
        {
            _sprite.FrameChanged -= OnFrameChanged;
            _sprite.AnimationFinished -= OnAnimationFinished;
        }

        if (_hitbox != null)
        {
            _hitbox.SetDeferred(Area2D.PropertyName.Monitoring, false);
            _hitbox.SetDeferred(Area2D.PropertyName.Monitorable, false);
        }
    }

    private void OnFrameChanged()
    {
        if (_sprite.Animation == _animation)
        {
            UpdateHitboxForFrame(_sprite.Frame);
        }
    }

    private void UpdateHitboxForFrame(int frame)
    {
        if (_hitbox == null)
        {
            return;
        }

        bool isActive = frame == _activeFrame;
        _hitbox.SetDeferred(Area2D.PropertyName.Monitoring, isActive);
        _hitbox.SetDeferred(Area2D.PropertyName.Monitorable, isActive);
    }

    private void OnAnimationFinished()
    {
        if (_sprite.Animation == _animation)
        {
            Transition(_onComplete);
        }
    }

    public override void UpdatePhysics(double delta)
    {
        if (_onFall != null && !_enemy.IsOnFloor())
        {
            Transition(_onFall);
            return;
        }

        _enemy.Decelerate(new Vector2(_deceleration * (float)delta, 0));
    }
}
