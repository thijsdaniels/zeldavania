using Godot;
using Zeldavania.Combat;

public partial class PlayerAttacking : State
{
    [Export]
    private CharacterBody2D _body;

    [Export]
    private AnimatedSprite2D _sprite;

    [Export]
    private Hitbox _hitbox;

    [Export]
    private CollisionShape2D _hitboxShape;

    [Export]
    private AudioStreamPlayer2D _soundEffect;

    [ExportGroup("Physics")]
    [Export]
    private float _deceleration = 800f;

    [Export]
    private float _gravity = 500f;

    [Export]
    private float _terminalVelocity = 220f;

    [Export]
    private float _forwardImpulse = 25f;

    [ExportGroup("Standing")]
    [Export]
    private State _standingState;

    [ExportGroup("Running")]
    [Export]
    private State _runningState;

    [ExportGroup("Falling")]
    [Export]
    private State _fallingState;

    private bool _isActive;
    private bool _hasBufferedAttack;

    public override void _Ready()
    {
        if (_body == null)
            GD.PushError($"[PlayerAttacking] '{GetPath()}' has no CharacterBody2D assigned!");
        if (_sprite == null)
            GD.PushError($"[PlayerAttacking] '{GetPath()}' has no AnimatedSprite2D assigned!");

        if (_sprite != null)
        {
            _sprite.AnimationFinished += HandleAnimationFinished;
        }

        if (_hitboxShape != null)
        {
            _hitboxShape.Disabled = true;
        }
    }

    public override void _ExitTree()
    {
        if (_sprite != null)
        {
            _sprite.AnimationFinished -= HandleAnimationFinished;
        }
    }

    public override void Enter()
    {
        _isActive = true;
        _hasBufferedAttack = false;

        if (_sprite != null)
        {
            _sprite.SpeedScale = 1.0f;
        }

        float facingDir = (_sprite != null && _sprite.FlipH) ? -1f : 1f;

        if (_hitbox != null)
        {
            _hitbox.Position = new Vector2(10f * facingDir, -7f);
        }

        if (_hitboxShape != null)
        {
            _hitboxShape.Disabled = true;
        }

        if (_body.IsOnFloor())
        {
            _body.Velocity = new Vector2(
                facingDir * _forwardImpulse,
                _body.Velocity.Y
            );
        }

        if (_sprite != null)
        {
            _sprite.Play("Sword");
            _sprite.Frame = 0;
        }

        _soundEffect?.Play();
    }

    public override void Exit()
    {
        _isActive = false;
        _hasBufferedAttack = false;

        if (_hitboxShape != null)
        {
            _hitboxShape.Disabled = true;
        }
    }

    public override void UpdatePhysics(double delta)
    {
        if (_sprite == null)
            return;

        // Hitbox active during swing frames 1, 2, 3
        if (_hitboxShape != null)
        {
            int frame = _sprite.Frame;
            _hitboxShape.Disabled = !(frame >= 1 && frame <= 3);
        }

        // Buffer follow-up attack during recovery phase
        if (_sprite.Frame >= 3 && Input.IsActionJustPressed(Controller.X))
        {
            _hasBufferedAttack = true;
        }

        // Movement physics
        if (_body.IsOnFloor())
        {
            _body.Velocity = new Vector2(
                Mathf.MoveToward(_body.Velocity.X, 0, _deceleration * (float)delta),
                _body.Velocity.Y
            );
        }
        else
        {
            if (_body.Velocity.Y < _terminalVelocity)
            {
                _body.Velocity += new Vector2(0, _gravity * (float)delta);
            }
        }

        // Fallback completion check if non-looping animation finished or reached last frame
        if (!_sprite.IsPlaying() && _sprite.Frame >= 4)
        {
            HandleAnimationFinished();
        }
    }

    private void HandleAnimationFinished()
    {
        if (!_isActive || _sprite.Animation != "Sword")
            return;

        if (_hasBufferedAttack)
        {
            Enter();
            return;
        }

        if (_body.IsOnFloor())
        {
            if (Controller.GetHorizontalDirection() != 0)
            {
                Transition(_runningState);
            }
            else
            {
                Transition(_standingState);
            }
        }
        else
        {
            Transition(_fallingState);
        }
    }
}
