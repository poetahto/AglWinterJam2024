using pt_player_3d.Scripts.Interaction;
using UnityEngine;

public class Bed : Interactable
{
    public override void Interact()
    {
        base.Interact(); 
        Game.Overworld.ReadyForNextDay = true;
    }

    public override bool CanInteract(GameObject source)
    {
        return base.CanInteract(source) && Game.Save.TimeOfDay == TimeOfDay.Night;
    }
}
