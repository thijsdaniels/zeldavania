using Godot;
using Zeldavania.Combat;

public partial class Player : CharacterBody2D
{
    [Export]
    private Damageable _damageable;

    [Export]
    private AudioStreamPlayer2D _deathSoundEffect;

    private Vector2 _spawnPosition;

    public override void _Ready()
    {
        _spawnPosition = GlobalPosition;

        if (_damageable != null)
        {
            _damageable.OnDepleted += HandleDefeat;
        }
    }

    public override void _ExitTree()
    {
        if (_damageable != null)
        {
            _damageable.OnDepleted -= HandleDefeat;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }

    private void HandleDefeat()
    {
        _deathSoundEffect?.Play();
        GlobalPosition = _spawnPosition;
        Velocity = Vector2.Zero;
        SetCollisionMaskValue(2, true);
        _damageable?.Reset();
    }
}
