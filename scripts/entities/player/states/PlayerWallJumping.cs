using Godot;

public partial class PlayerWallJumping : State
{
    [Export]
    private CharacterBody2D _body;

    [Export]
    private Player _player;

    [Export]
    private AnimatedSprite2D _sprite;

    [ExportGroup("Wall Jump Parameters")]
    [Export]
    private float _verticalImpulse = 200f;

    [Export]
    private float _horizontalImpulse = 130f;

    [Export]
    private float _lockoutDuration = 0.15f;

    [Export]
    private AudioStreamPlayer2D _soundEffect;

    [ExportGroup("Transitions")]
    [Export]
    private State _fallingState;

    private float _wallNormalX = 1f;

    public void SetWallNormal(float normalX)
    {
        _wallNormalX = normalX != 0 ? Mathf.Sign(normalX) : 1f;
    }

    public override void Enter()
    {
        _soundEffect?.Play();

        _body.Velocity = new Vector2(
            _wallNormalX * _horizontalImpulse,
            -_verticalImpulse
        );

        if (_fallingState is PlayerFalling falling)
        {
            falling.SetInputLockout(_lockoutDuration, -_wallNormalX);
        }

        Transition(_fallingState);
    }
}
