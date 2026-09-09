using Godot;
using Zeldavania.Inventory;

public partial class PlayerSwimming : State
{
    [Export]
    private CharacterBody2D _body;

    [Export]
    private Player _player;

    [Export]
    private AnimatedSprite2D _sprite;

    [ExportGroup("Swimming")]
    [Export]
    private WaterDetector2D _waterDetector;

    [Export]
    private WaterfallDetector2D _waterfallDetector;

    [Export]
    private float _surfaceSwimSpeed = 65f;

    [Export]
    private float _acceleration = 500f;

    [Export]
    private float _deceleration = 600f;

    [Export]
    private float _leapVelocity = 190f;

    [Export]
    private AudioStreamPlayer2D _splashEffect;

    [ExportGroup("Buoyancy Physics")]
    [Export]
    private float _buoyancySpringStiffness = 80f;

    [Export]
    private float _buoyancyDamping = 6.5f;

    [Export]
    private float _waterlineOffset = 8f;

    [Export]
    private float _maxBuoyancyAscentSpeed = 90f;

    [ExportGroup("Diving")]
    [Export]
    private float _shallowDiveDuration = 0.65f;

    [Export]
    private float _deepDiveDuration = 1.3f;

    [Export]
    private float _shallowDiveDownwardForce = 2800f;

    [Export]
    private float _deepDiveDownwardForce = 5600f;

    [Export]
    private float _maxDepthHoldDuration = 0.5f;

    [Export]
    private float _diveCooldown = 0.5f;

    [ExportGroup("Transitions")]
    [Export]
    private State _fallingState;

    [Export]
    private State _divingState;

    private float _diveDescentTimer;
    private float _diveHoldTimer;
    private float _diveCooldownTimer;
    private bool _isDiving;
    private bool _isResurfacing;

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
            _sprite.Play("Idle");
        }

        // Clean plunge entry preserving 75% of downward momentum
        _body.Velocity = new Vector2(_body.Velocity.X * 0.75f, Mathf.Clamp(_body.Velocity.Y * 0.75f, -80f, 160f));
        _diveCooldownTimer = 0f;
        _isDiving = false;
        _isResurfacing = false;

        if (_waterDetector != null)
            _waterDetector.OnTileExited += OnWaterExited;
    }

    public override void Exit()
    {
        _isDiving = false;
        _isResurfacing = false;
        if (_waterDetector != null)
            _waterDetector.OnTileExited -= OnWaterExited;
    }

    private void OnWaterExited()
    {
        if (_sprite != null)
        {
            _sprite.ZIndex = 0;
        }

        if (_body.Velocity.Y < 0 && _fallingState is PlayerFalling falling)
        {
            falling.NotifyJumpAscent();
        }

        Transition(_fallingState);
    }

    public override void UpdatePhysics(double delta)
    {
        if (_waterDetector == null || !_waterDetector.IsOverlapping)
        {
            OnWaterExited();
            return;
        }

        // 1. Dynamic waterline lookup
        float targetY = _body.GlobalPosition.Y;
        bool isUnderCeiling = false;
        if (_waterDetector.TryGetTopSurfaceY(_body.GlobalPosition, out float topSurfaceY, out isUnderCeiling))
        {
            targetY = topSurfaceY + _waterlineOffset;
        }

        float deltaY = _body.GlobalPosition.Y - targetY;
        int scaleTier = _player?.Inventory?.GetPassiveTier("scale") ?? 0;

        // Check if resurfacing completed (reached open waterline)
        if (_isResurfacing)
        {
            if (!isUnderCeiling && deltaY <= 2f)
            {
                _isResurfacing = false;
                _diveCooldownTimer = _diveCooldown;
            }
        }
        else if (_diveCooldownTimer > 0)
        {
            _diveCooldownTimer -= (float)delta;
        }

        // 2. Active Dive initiation & duration management
        if (Input.IsActionPressed(Controller.Down))
        {
            if (scaleTier >= 3 && _divingState != null)
            {
                // Flippers (Tier 3+): transition directly into 360° free-form diving!
                _body.Velocity = new Vector2(_body.Velocity.X, 40f);
                Transition(_divingState);
                return;
            }

            bool canDive = scaleTier >= 1 && !_isDiving && !_isResurfacing && _diveCooldownTimer <= 0 && !isUnderCeiling && deltaY <= 4f;
            if (canDive && !_isDiving)
            {
                _isDiving = true;
                _diveDescentTimer = (scaleTier >= 2) ? _deepDiveDuration : _shallowDiveDuration;
                _diveHoldTimer = _maxDepthHoldDuration;
            }
        }

        if (_isDiving)
        {
            if (!Input.IsActionPressed(Controller.Down) || _body.IsOnFloor())
            {
                EndDive();
            }
            else if (_diveDescentTimer > 0)
            {
                _diveDescentTimer -= (float)delta;
            }
            else if (_diveHoldTimer > 0)
            {
                _diveHoldTimer -= (float)delta;
            }
            else
            {
                EndDive();
            }
        }

        // 3. Active Breach Jump out of water (only available when near surface and not diving)
        if (!_isDiving && deltaY <= 4f && Input.IsActionJustPressed(Controller.A) && !Input.IsActionPressed(Controller.Down))
        {
            BreachJump();
            return;
        }

        // 4. Horizontal movement & waterfall plunge dispersal
        float hDir = Controller.GetHorizontalDirection();
        if (_waterfallDetector != null && _waterfallDetector.TryGetWaterfallPlungeDispersal(_body.GlobalPosition, out float pushSpeed) && pushSpeed != 0f)
        {
            float targetHVel = (hDir * _surfaceSwimSpeed) + pushSpeed;
            float finalHVel = Mathf.MoveToward(_body.Velocity.X, targetHVel, _acceleration * (float)delta);
            _body.Velocity = new Vector2(finalHVel, _body.Velocity.Y);
        }
        else
        {
            _body.MoveWithInertia(
                direction: hDir,
                acceleration: _acceleration * (float)delta,
                deceleration: _deceleration * (float)delta,
                limit: _surfaceSwimSpeed
            );
        }

        // 5. Vertical Fluid Physics: Spring-Damper Buoyancy + Dive Forces
        float springForce = -_buoyancySpringStiffness * deltaY;
        float dampingForce = -_buoyancyDamping * _body.Velocity.Y;
        float netVerticalAcc = springForce + dampingForce;

        if (_isDiving)
        {
            if (_diveDescentTimer > 0)
            {
                // Active downward swimming thrust fighting buoyancy
                float activeDiveForce = (scaleTier >= 2) ? _deepDiveDownwardForce : _shallowDiveDownwardForce;
                netVerticalAcc += activeDiveForce;
            }
            else if (_diveHoldTimer > 0)
            {
                // Neutral hover holding max depth against spring
                netVerticalAcc = -_buoyancyDamping * _body.Velocity.Y;
                _body.Velocity = new Vector2(_body.Velocity.X, Mathf.MoveToward(_body.Velocity.Y, 0f, 400f * (float)delta));
            }
        }

        float newVy = _body.Velocity.Y + (netVerticalAcc * (float)delta);

        // Cap velocities for smooth control
        if (_isDiving)
        {
            float maxDiveSpeed = (scaleTier >= 2) ? 100f : 80f;
            newVy = Mathf.Min(newVy, maxDiveSpeed);
        }
        else
        {
            newVy = Mathf.Max(newVy, -_maxBuoyancyAscentSpeed);
        }

        _body.Velocity = new Vector2(_body.Velocity.X, newVy);

        // 6. Visual Animation
        if (_isDiving)
        {
            _sprite?.Play("Fall");
        }
        else if (hDir != 0)
        {
            _sprite?.Play("Swim");
        }
        else
        {
            _sprite?.Play("Idle");
        }

        _sprite?.SynchronizeAnimation(-new Vector2(hDir, 0));
    }

    private void EndDive()
    {
        _isDiving = false;
        _isResurfacing = true;
        _diveDescentTimer = 0;
        _diveHoldTimer = 0;
        _diveCooldownTimer = 0;
    }

    private void BreachJump()
    {
        _splashEffect?.Play();
        _body.Velocity = new Vector2(_body.Velocity.X, -_leapVelocity);

        if (_sprite != null)
        {
            _sprite.ZIndex = 0;
        }

        if (_fallingState is PlayerFalling falling)
        {
            falling.NotifyJumpAscent();
        }

        Transition(_fallingState);
    }
}
