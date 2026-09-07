using Godot;
using Zeldavania.UserInterface;
using Zeldavania.Utilities;

namespace Zeldavania.Objects;

[GlobalClass]
public partial class Signpost : Node2D
{
    [Export(PropertyHint.MultilineText)]
    public string Message { get; set; } = "A wooden signpost stands firmly here.";

    [Export]
    private Interactable2D _interactable;

    public override void _Ready()
    {
        if (_interactable == null)
        {
            _interactable = GetNodeOrNull<Interactable2D>("Interactable2D");
        }

        if (_interactable != null)
        {
            _interactable.Interacted += HandleInteracted;
        }
    }

    public override void _ExitTree()
    {
        if (_interactable != null)
        {
            _interactable.Interacted -= HandleInteracted;
        }
    }

    private void HandleInteracted(Player player)
    {
        var dialogueBox = FindDialogueBox();
        if (dialogueBox != null)
        {
            dialogueBox.Open(Message, () =>
            {
                var interactingState = player?.GetNodeOrNull<PlayerInteracting>("State/Interacting");
                interactingState?.CompleteInteraction();
            });
        }
        else
        {
            // Fallback if no DialogueBox is present
            var interactingState = player?.GetNodeOrNull<PlayerInteracting>("State/Interacting");
            interactingState?.CompleteInteraction();
        }
    }

    private DialogueBox FindDialogueBox()
    {
        var root = GetTree()?.Root;
        if (root == null)
        {
            return null;
        }

        return root.FindChild("DialogueBox", recursive: true, owned: false) as DialogueBox;
    }
}
