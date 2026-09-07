using Godot;
using Zeldavania.Inventory;

public partial class PlayerAiming : State
{
    [Export]
    private CharacterBody2D _body;

    [Export]
    private AnimatedSprite2D _sprite;

    [Export]
    private Node2D _crosshair;

    [ExportGroup("Movement")]
    [Export]
    private float _walkAimSpeed = 35f;

    [Export]
    private float _deceleration = 800f;

    [Export]
    private float _gravity = 500f;

    [Export]
    private float _terminalVelocity = 220f;

    [ExportGroup("Aiming")]
    [Export]
    private float _timeScale = 0.3f;

    [Export]
    private float _sweepSpeed = 2.5f;

    [Export]
    private float _minElevation = -0.785398f; // -45 degrees

    [Export]
    private float _maxElevation = 1.570796f; // +90 degrees

    [Export]
    private float _crosshairDistance = 24f;

    [Export]
    private Vector2 _crosshairOffset = new Vector2(0, -12f);

    [ExportGroup("Shooting")]
    [Export]
    private PlayerShooting _shootingState;

    public static float CurrentElevation { get; set; } = 0f;

    public string TriggerAction { get; set; } = Controller.B;

    public override void _Ready()
    {
        if (_crosshair != null)
        {
            _crosshair.Visible = false;
        }
    }

    public override void Enter()
    {
        Engine.TimeScale = _timeScale;

        if (_sprite != null)
        {
            _sprite.Play("Shoot");
            _sprite.Frame = 2;
            _sprite.Pause();
        }

        if (_crosshair != null)
        {
            _crosshair.Visible = true;
            UpdateCrosshairPosition();
        }
    }

    public override void Exit()
    {
        Engine.TimeScale = 1.0;

        if (_crosshair != null)
        {
            _crosshair.Visible = false;
        }
    }

    public override void UpdatePhysics(double delta)
    {
        float realDelta = (float)delta / Mathf.Max(0.01f, (float)Engine.TimeScale);

        if (Input.IsActionPressed(Controller.Up))
        {
            CurrentElevation = Mathf.Min(_maxElevation, CurrentElevation + _sweepSpeed * realDelta);
        }
        else if (Input.IsActionPressed(Controller.Down))
        {
            CurrentElevation = Mathf.Max(_minElevation, CurrentElevation - _sweepSpeed * realDelta);
        }

        float hDir = Controller.GetHorizontalDirection();
        if (hDir != 0)
        {
            if (_sprite != null)
            {
                _sprite.FlipH = hDir < 0;
            }

            if (_body.IsOnFloor())
            {
                _body.Velocity = new Vector2(hDir * _walkAimSpeed, _body.Velocity.Y);
            }
        }
        else if (_body.IsOnFloor())
        {
            _body.Velocity = new Vector2(
                Mathf.MoveToward(_body.Velocity.X, 0, _deceleration * (float)delta),
                _body.Velocity.Y
            );
        }

        if (!_body.IsOnFloor())
        {
            if (_body.Velocity.Y < _terminalVelocity)
            {
                _body.Velocity += new Vector2(0, _gravity * (float)delta);
            }
        }

        UpdateCrosshairPosition();

        if (!Input.IsActionPressed(TriggerAction))
        {
            float facingDir = (_sprite != null && _sprite.FlipH) ? -1f : 1f;
            Vector2 aimDir = new Vector2(
                facingDir * Mathf.Cos(CurrentElevation),
                -Mathf.Sin(CurrentElevation)
            ).Normalized();

            if (_shootingState != null)
            {
                _shootingState.AimDirection = aimDir;
                Transition(_shootingState);
            }
        }
    }

    private void UpdateCrosshairPosition()
    {
        if (_crosshair == null || _body == null)
            return;

        float facingDir = (_sprite != null && _sprite.FlipH) ? -1f : 1f;
        Vector2 aimDir = new Vector2(
            facingDir * Mathf.Cos(CurrentElevation),
            -Mathf.Sin(CurrentElevation)
        ).Normalized();

        _crosshair.GlobalPosition = _body.GlobalPosition + _crosshairOffset + (aimDir * _crosshairDistance);
    }
}
