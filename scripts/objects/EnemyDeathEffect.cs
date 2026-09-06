using Godot;

public partial class EnemyDeathEffect : Node2D
{
    [Export]
    private AnimatedSprite2D _sprite;

    [Export]
    private AudioStreamPlayer2D _audio;

    [Export]
    private float _minPitch = 0.95f;

    [Export]
    private float _maxPitch = 1.15f;

    public override void _Ready()
    {
        if (_audio != null)
        {
            _audio.PitchScale = (float)GD.RandRange(_minPitch, _maxPitch);
            _audio.Play();
        }

        if (_sprite != null)
        {
            _sprite.AnimationFinished += OnAnimationFinished;
            _sprite.Play("default");
        }
    }

    private void OnAnimationFinished()
    {
        if (_sprite != null)
        {
            _sprite.AnimationFinished -= OnAnimationFinished;
        }

        QueueFree();
    }
}
