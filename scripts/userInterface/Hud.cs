using Godot;
using Zeldavania.Combat;
using Zeldavania.Inventory;

namespace Zeldavania.UserInterface;

[GlobalClass]
public partial class Hud : CanvasLayer
{
    [Export]
    private Damageable _playerDamageable;

    [Export]
    private Inventory.Inventory _playerInventory;

    [Export]
    private HealthBar _healthBar;

    [Export]
    private ItemActionCluster _actionCluster;

    [Export]
    private DialogueBox _dialogueBox;

    public DialogueBox DialogueBox => _dialogueBox;

    public void ShowDialogue(string message, System.Action onClosed = null)
    {
        _dialogueBox?.Open(message, onClosed);
    }

    public void CloseDialogue()
    {
        _dialogueBox?.Close();
    }

    public override void _Ready()
    {
        if (_playerDamageable != null)
        {
            BindHealth(_playerDamageable);
        }

        if (_playerInventory != null)
        {
            BindInventory(_playerInventory);
        }
    }

    public override void _ExitTree()
    {
        if (_playerDamageable != null)
        {
            _playerDamageable.OnHealthChanged -= HandleHealthChanged;
        }
    }

    public void Bind(Damageable damageable, Inventory.Inventory inventory = null)
    {
        BindHealth(damageable);
        if (inventory != null)
        {
            BindInventory(inventory);
        }
    }

    public void BindHealth(Damageable damageable)
    {
        if (_playerDamageable != null)
        {
            _playerDamageable.OnHealthChanged -= HandleHealthChanged;
        }

        _playerDamageable = damageable;

        if (_playerDamageable != null)
        {
            _playerDamageable.OnHealthChanged += HandleHealthChanged;
            _healthBar?.UpdateHealth(_playerDamageable.CurrentHitPoints, _playerDamageable.MaxHitPoints);
        }
    }

    public void BindInventory(Inventory.Inventory inventory)
    {
        _playerInventory = inventory;
        if (_playerInventory != null)
        {
            _actionCluster?.Bind(_playerInventory);
        }
    }

    private void HandleHealthChanged(int currentHitPoints, int maxHitPoints)
    {
        _healthBar?.UpdateHealth(currentHitPoints, maxHitPoints);
    }
}
