namespace Zeldavania.Inventory;

public interface IActionTriggerable
{
    bool TryInitialize(Player player, string actionButton);
}
