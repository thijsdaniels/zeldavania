using Godot;

namespace Zeldavania.Combat;

[GlobalClass]
public partial class Damageable : Node
{
    [Signal]
    public delegate void OnDamagedEventHandler(int amount, Vector2 origin);

    [Signal]
    public delegate void OnHealthChangedEventHandler(int currentHitPoints, int maxHitPoints);

    [Signal]
    public delegate void OnDepletedEventHandler();

    [Export]
    public int MaxHitPoints { get; set; } = 1;

    [Export]
    public int CurrentHitPoints { get; set; } = 1;

    public bool IsDepleted => CurrentHitPoints <= 0;

    public override void _Ready()
    {
        CurrentHitPoints = Mathf.Clamp(CurrentHitPoints, 0, MaxHitPoints);
    }

    public void TakeDamage(Hit hit)
    {
        if (IsDepleted || hit.Damage <= 0)
        {
            return;
        }

        CurrentHitPoints = Mathf.Max(0, CurrentHitPoints - hit.Damage);
        EmitSignal(SignalName.OnDamaged, hit.Damage, hit.Origin);
        EmitSignal(SignalName.OnHealthChanged, CurrentHitPoints, MaxHitPoints);

        if (IsDepleted)
        {
            EmitSignal(SignalName.OnDepleted);
        }
    }

    public void Heal(int amount)
    {
        if (IsDepleted || amount <= 0)
        {
            return;
        }

        CurrentHitPoints = Mathf.Min(MaxHitPoints, CurrentHitPoints + amount);
        EmitSignal(SignalName.OnHealthChanged, CurrentHitPoints, MaxHitPoints);
    }

    public void Reset()
    {
        CurrentHitPoints = MaxHitPoints;
        EmitSignal(SignalName.OnHealthChanged, CurrentHitPoints, MaxHitPoints);
    }
}
