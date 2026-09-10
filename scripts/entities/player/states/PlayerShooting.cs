using Godot;
using Zeldavania.Inventory;

public partial class PlayerShooting : State
{
    [Export]
    private CharacterBody2D _body;

    [Export]
    private AnimatedSprite2D _sprite;

    [Export]
    private PackedScene _arrowScene;

    [Export]
    private BowItem _bowItem;

    [Export]
    private Vector2 _muzzleOffset = new Vector2(8f, -12f);

    [Export]
    private AudioStreamPlayer2D _soundEffect;

    [ExportGroup("Physics")]
    [Export]
    private float _deceleration = 800f;

    [Export]
    private float _gravity = 500f;

    [Export]
    private float _terminalVelocity = 220f;

    [ExportGroup("Standing")]
    [Export]
    private State _standingState;

    [ExportGroup("Running")]
    [Export]
    private State _runningState;

    [ExportGroup("Falling")]
    [Export]
    private State _fallingState;

    public Vector2 AimDirection { get; set; } = Vector2.Zero;

    private bool _isActive;
    private bool _arrowSpawned;

    public override void _Ready()
    {
        if (_sprite != null)
        {
            _sprite.AnimationFinished += HandleAnimationFinished;
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
        _arrowSpawned = false;

        if (AimDirection == Vector2.Zero)
        {
            float facingDir = (_sprite != null && _sprite.FlipH) ? -1f : 1f;
            AimDirection = new Vector2(
                facingDir * Mathf.Cos(PlayerAiming.CurrentElevation),
                -Mathf.Sin(PlayerAiming.CurrentElevation)
            ).Normalized();
        }

        SpawnArrow();

        if (_sprite != null)
        {
            _sprite.SpeedScale = 1.5f;
            _sprite.Play("Shoot");
            _sprite.Frame = 3;
        }
    }

    public override void Exit()
    {
        _isActive = false;
        _arrowSpawned = false;
        AimDirection = Vector2.Zero;

        if (_sprite != null)
        {
            _sprite.SpeedScale = 1.0f;
        }
    }

    public override void UpdatePhysics(double delta)
    {
        if (_body.IsOnFloor())
        {
            _body.Velocity = new Vector2(
                Mathf.MoveToward(
                    _body.Velocity.X,
                    0,
                    _deceleration * (float)delta
                ),
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

        if (_sprite != null && !_sprite.IsPlaying() && _sprite.Frame >= 4)
        {
            HandleAnimationFinished();
        }
    }

    private void SpawnArrow()
    {
        if (_arrowSpawned || _arrowScene == null || _body == null)
        {
            return;
        }

        _arrowSpawned = true;

        if (_bowItem != null)
        {
            var inventory = _bowItem.GetParent() as Inventory;
            if (inventory != null)
            {
                inventory.ConsumeAmmo(_bowItem, 1);
            }
            else
            {
                _bowItem.CurrentAmmo = Mathf.Max(0, _bowItem.CurrentAmmo - 1);
            }
        }

        var arrowInstance = _arrowScene.Instantiate<Arrow>();
        if (arrowInstance != null)
        {
            if (_bowItem != null && _bowItem.ActiveVariant == ArrowVariant.Fire)
            {
                arrowInstance.Ignite();
            }

            float facingDir = (_sprite != null && _sprite.FlipH) ? -1f : 1f;
            Vector2 spawnPos =
                _body.GlobalPosition
                + new Vector2(_muzzleOffset.X * facingDir, _muzzleOffset.Y);

            var treeRoot = _body.GetTree().CurrentScene ?? _body.GetParent();
            treeRoot.AddChild(arrowInstance);
            arrowInstance.GlobalPosition = spawnPos;

            arrowInstance.Launch(AimDirection);
            _soundEffect?.Play();
        }
    }

    private void HandleAnimationFinished()
    {
        if (!_isActive || _sprite == null || _sprite.Animation != "Shoot")
        {
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
