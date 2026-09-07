using Godot;

namespace Zeldavania.Utilities;

[GlobalClass]
public partial class Interactable2D : Area2D
{
    [Signal]
    public delegate void InteractedEventHandler(Player player);

    [Signal]
    public delegate void PlayerEnteredEventHandler(Player player);

    [Signal]
    public delegate void PlayerExitedEventHandler(Player player);

    [Export]
    public bool IsActive { get; set; } = true;

    [Export]
    public string PromptAction { get; set; } = "Read";

    [Export]
    private Node2D _promptCue;

    public override void _Ready()
    {
        SetPromptVisible(false);
    }

    public void NotifyPlayerEntered(Player player)
    {
        if (!IsActive)
        {
            return;
        }

        SetPromptVisible(true);
        EmitSignal(SignalName.PlayerEntered, player);
    }

    public void NotifyPlayerExited(Player player)
    {
        SetPromptVisible(false);
        EmitSignal(SignalName.PlayerExited, player);
    }

    public void Interact(Player player)
    {
        if (IsActive)
        {
            EmitSignal(SignalName.Interacted, player);
        }
    }

    public void SetPromptVisible(bool visible)
    {
        if (_promptCue != null)
        {
            _promptCue.Visible = visible;
        }
    }
}
