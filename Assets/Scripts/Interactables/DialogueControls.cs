using pt_player_3d.Scripts.Interaction;
using UnityEngine;

public class DialogueControls : Interactable
{
    [SerializeField] private DialogueSystem dialogueSystem;
    
    private bool _dialogueOptionActive;

    public override void Interact()
    {
        base.Interact();

        if (dialogueSystem.HasOptions())
            dialogueSystem.Open();
    }

    public override bool CanInteract(GameObject source)
    {
        return base.CanInteract(source) && dialogueSystem.HasOptions() && !dialogueSystem.IsOpen();
    }
}
