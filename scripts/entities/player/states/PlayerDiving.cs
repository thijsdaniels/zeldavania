using Godot;
using Zeldavania.Inventory;

public partial class PlayerDiving : State
{
    [Export]
    private CharacterBody2D _body;

    [Export]
    private Player _player;

    [Export]
    private AnimatedSprite2D _sprite;

    [ExportGroup("Detection")]
    [Export]
    private WaterDetector2D _waterDetector;

    [Export]
    private WaterfallDetector2D _waterfallDetector;

    [ExportGroup("Diving Physics")]
    [Export]
    private float _freeSwimSpeed = 110f;

    [Export]
    private float _flipperDashSpeed = 210f;

    [Export]
    private float _acceleration = 700f;

    [Export]
    private float _deceleration = 500f;

    [Export]
    private float _rotationSmoothness = 16f;

    [Export]
    private float _dolphinVerticalVelocity = 195f;

    [Export]
    private float _dolphinHorizontalVelocity = 125f;

    [Export]
    private float _dolphinDashMultiplier = 1.35f;

    [Export]
    private AudioStreamPlayer2D _splashEffect;

    [ExportGroup("Currents & Waterfalls")]
    [Export]
    private float _waterfallCurrentForce = 600f;

    [ExportGroup("Transitions")]
    [Export]
    private State _fallingState;

    [Export]
    private State _swimmingState;

    private float _headingAngle = Mathf.Pi * 0.5f;

    public override void _Ready()
    {
        if (_player == null && _body is Player player)
        {
            _player = player;
        }
    }

    public override void Enter()
    {
        if (_player == null && _body is Player player)
        {
            _player = player;
        }

        _splashEffect?.Play();
        if (_sprite != null)
        {
            _sprite.ZIndex = -1;
            _sprite.Play("Swim");
        }

        // Initialize heading based on initial plunge velocity or facing direction
        if (_body.Velocity.LengthSquared() > 100f)
        {
            _headingAngle = _body.Velocity.Angle();
        }
        else
        {
            _headingAngle = Mathf.Pi * 0.5f; // Pointing downward into water
        }

        if (_sprite != null)
        {
            _sprite.Rotation = _headingAngle + (Mathf.Pi * 0.5f);
            _sprite.FlipV = false;
            _sprite.FlipH = false;
        }

        if (_waterDetector != null)
            _waterDetector.OnTileExited += OnCheckWaterExited;
        if (_waterfallDetector != null)
            _waterfallDetector.OnTileExited += OnCheckWaterExited;
    }

    public override void Exit()
    {
        if (_sprite != null)
        {
            _sprite.Rotation = 0f;
            _sprite.FlipV = false;
            _sprite.FlipH = false;
            _sprite.ZIndex = 0;
        }

        if (_waterDetector != null)
            _waterDetector.OnTileExited -= OnCheckWaterExited;
        if (_waterfallDetector != null)
            _waterfallDetector.OnTileExited -= OnCheckWaterExited;
    }

    private void OnCheckWaterExited()
    {
        if (!IsInsideWater())
        {
            if (_sprite != null)
            {
                _sprite.Rotation = 0f;
                _sprite.FlipV = false;
                _sprite.FlipH = false;
                _sprite.ZIndex = 0;
            }

            if (_body.Velocity.Y < 0 && _fallingState is PlayerFalling falling)
            {
                falling.NotifyJumpAscent();
            }

            Transition(_fallingState);
        }
    }

    public override void UpdatePhysics(double delta)
    {
        if (!IsInsideWater())
        {
            OnCheckWaterExited();
            return;
        }

        int swimTier = _player?.Inventory?.GetPassiveTier("scale") ?? 0;

        // Route unequipped players (tier < 3) to Swimming (in pool) or Falling (in waterfall/air)
        if (swimTier < 3)
        {
            if (_waterDetector != null && _waterDetector.IsOverlapping && _swimmingState != null)
            {
                Transition(_swimmingState);
                return;
            }
            if (_fallingState != null)
            {
                Transition(_fallingState);
                return;
            }
        }

        // 1. Waterfall / Current handling
        if (IsInsideWaterfall())
        {
            HandleWaterfallPhysics(delta, swimTier);
            return;
        }

        // 2. Direct 360-degree analog/directional steering (Rayman-style)
        Vector2 inputDir = Controller.GetDirection();
        if (inputDir != Vector2.Zero)
        {
            _headingAngle = inputDir.Angle();
        }

        Vector2 headingVector = new Vector2(Mathf.Cos(_headingAngle), Mathf.Sin(_headingAngle));

        // 3. Movement: Direct movement or dash in input/last-heading direction
        bool isDashing = (swimTier >= 4 && Input.IsActionPressed(Controller.A));
        Vector2 targetVelocity = Vector2.Zero;

        if (isDashing)
        {
            // Dash in active input direction, or in last heading direction if no stick input is held
            Vector2 dashDir = inputDir != Vector2.Zero ? inputDir.Normalized() : headingVector;
            targetVelocity = dashDir * _flipperDashSpeed;
        }
        else if (inputDir != Vector2.Zero)
        {
            targetVelocity = inputDir.Normalized() * _freeSwimSpeed;
        }

        if (targetVelocity != Vector2.Zero)
        {
            _body.Velocity = _body.Velocity.MoveToward(targetVelocity, _acceleration * (float)delta);
            _sprite?.Play("Swim");
        }
        else
        {
            _body.Velocity = _body.Velocity.MoveToward(Vector2.Zero, _deceleration * (float)delta);
            _sprite?.Play("Idle");
        }

        // 4. Surface waterline & Dolphin Leap check
        if (_waterDetector != null && _waterDetector.TryGetTopSurfaceY(_body.GlobalPosition, out float topSurfaceY, out bool isUnderCeiling))
        {
            float waterlineY = topSurfaceY + 8f;

            if (!isUnderCeiling && _body.GlobalPosition.Y <= waterlineY)
            {
                // Check if emerging with upward velocity or holding upward/jump input -> DOLPHIN LEAP!
                bool isEmergingUpward = _body.Velocity.Y < -30f
                    || (headingVector.Y < -0.1f && _body.Velocity.Length() > 40f)
                    || Input.IsActionJustPressed(Controller.A);

                if (isEmergingUpward)
                {
                    DolphinLeap(isDashing, headingVector);
                    return;
                }

                // Smooth return to surface swimming when arriving gently
                if (_body.Velocity.Y <= 0 && inputDir.Y <= 0.2f && _swimmingState != null)
                {
                    Transition(_swimmingState);
                    return;
                }
            }
        }

        // 5. Visual Smooth Heading Rotation
        UpdateSpriteRotation(delta);
    }

    private void UpdateSpriteRotation(double delta)
    {
        if (_sprite == null) return;

        float targetVisualAngle = _headingAngle + (Mathf.Pi * 0.5f);
        float angleDiff = Mathf.AngleDifference(_sprite.Rotation, targetVisualAngle);
        _sprite.Rotation += angleDiff * Mathf.Min(1f, _rotationSmoothness * (float)delta);
        _sprite.FlipV = false;
        _sprite.FlipH = false;
    }

    private void DolphinLeap(bool isDashing, Vector2 headingVector)
    {
        _splashEffect?.Play();

        float multiplier = isDashing ? _dolphinDashMultiplier : 1.0f;

        // Shape vertical launch velocity so that diagonal breaches (e.g. 45 deg) get full vertical clearance (~195 px/s),
        // while preventing straight-up launches from rocketing too high.
        float upwardFactor = Mathf.Clamp(Mathf.Abs(headingVector.Y) + 0.35f, 0.75f, 1.0f);
        float vY = -_dolphinVerticalVelocity * upwardFactor * multiplier;

        // Apply robust horizontal momentum scaling with heading direction
        float vX = headingVector.X * _dolphinHorizontalVelocity * multiplier;

        _body.Velocity = new Vector2(vX, vY);

        if (_sprite != null)
        {
            _sprite.Rotation = 0f;
            _sprite.FlipV = false;
            _sprite.FlipH = false;
            _sprite.ZIndex = 0;
        }

        // Do not call NotifyJumpAscent() so upward hydrodynamic momentum is not truncated by jump-cut logic
        Transition(_fallingState);
    }

    private void HandleWaterfallPhysics(double delta, int swimTier)
    {
        Vector2 inputDir = Controller.GetDirection();
        if (inputDir != Vector2.Zero)
        {
            _headingAngle = inputDir.Angle();
        }

        Vector2 headingVector = new Vector2(Mathf.Cos(_headingAngle), Mathf.Sin(_headingAngle));

        if (swimTier >= 4 && (Input.IsActionPressed(Controller.Up) || Input.IsActionPressed(Controller.A)))
        {
            // Climb waterfall column
            _body.Velocity = new Vector2(inputDir.X * _freeSwimSpeed, -_flipperDashSpeed);
            _sprite?.Play("Swim");
        }
        else
        {
            float hInput = inputDir.X;
            bool touchingWaterPool = _waterDetector != null && _waterDetector.IsOverlapping;

            if (touchingWaterPool)
            {
                float pushSpeed = 0f;
                if (_waterfallDetector != null)
                {
                    _waterfallDetector.TryGetWaterfallPlungeDispersal(_body.GlobalPosition, out pushSpeed);
                }

                float targetHVel = (hInput * _freeSwimSpeed) + pushSpeed;
                float newHVel = Mathf.MoveToward(_body.Velocity.X, targetHVel, 900f * (float)delta);

                // Water cushions downward plunge momentum smoothly toward gentle settling speed
                float targetVVel = 20f;
                float newVVel = Mathf.MoveToward(_body.Velocity.Y, targetVVel, 600f * (float)delta);

                _body.Velocity = new Vector2(newHVel, newVVel);
                _sprite?.Play("Swim");
            }
            else
            {
                // Falling through waterfall column in air: heavy downward acceleration
                _body.Velocity += new Vector2(0, _waterfallCurrentForce * (float)delta);
                _body.Velocity = new Vector2(
                    hInput * (_freeSwimSpeed * 0.4f),
                    Mathf.Min(_body.Velocity.Y, 340f)
                );
                _sprite?.Play("Fall");
            }
        }

        UpdateSpriteRotation(delta);
    }

    private bool IsInsideWaterfall()
    {
        return _waterfallDetector != null && _waterfallDetector.IsOverlapping;
    }

    private bool IsInsideWater()
    {
        return (_waterDetector != null && _waterDetector.IsOverlapping) || IsInsideWaterfall();
    }
}
