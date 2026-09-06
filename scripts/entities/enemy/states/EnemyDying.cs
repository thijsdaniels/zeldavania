using Godot;

public partial class EnemyDying : State
{
    [Export]
    public Enemy _enemy;

    [Export]
    public AnimatedSprite2D _sprite;

    [Export]
    public string _animation = "Dying";

    [Export]
    public float _friction = 300f;

    [Export]
    public float _gravity = 500f;

    public override void Enter()
    {
        if (_sprite != null)
        {
            _sprite.AnimationFinished += OnAnimationFinished;
            if (_animation != null)
            {
                _sprite.Play(_animation);
            }
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

        _enemy?.QueueFree();
    }
}
