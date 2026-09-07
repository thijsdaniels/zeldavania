using System.Collections.Generic;
using Godot;

namespace Zeldavania.Utilities;

[GlobalClass]
public partial class InteractionDetector2D : Area2D
{
    [Export]
    private Player _player;

    private readonly List<Interactable2D> _interactables = new();

    public Interactable2D CurrentInteractable =>
        _interactables.Count > 0 ? _interactables[0] : null;

    public bool CanInteract =>
        CurrentInteractable != null && CurrentInteractable.IsActive;

    public override void _Ready()
    {
        if (_player == null && Owner is Player ownerPlayer)
        {
            _player = ownerPlayer;
        }

        AreaEntered += HandleAreaEntered;
        AreaExited += HandleAreaExited;
    }

    public override void _ExitTree()
    {
        AreaEntered -= HandleAreaEntered;
        AreaExited -= HandleAreaExited;
    }

    private void HandleAreaEntered(Area2D area)
    {
        if (area is Interactable2D interactable && !_interactables.Contains(interactable))
        {
            _interactables.Add(interactable);
            interactable.NotifyPlayerEntered(_player);
        }
    }

    private void HandleAreaExited(Area2D area)
    {
        if (area is Interactable2D interactable && _interactables.Contains(interactable))
        {
            _interactables.Remove(interactable);
            interactable.NotifyPlayerExited(_player);
        }
    }

    public void TriggerInteraction()
    {
        if (CanInteract)
        {
            CurrentInteractable.Interact(_player);
        }
    }
}
