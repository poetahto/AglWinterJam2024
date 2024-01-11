using pt_player_3d.Scripts.Interaction;

public class Bed : Interactable
{
    public override void Interact()
    {
        base.Interact(); 
        Game.Overworld.ReadyForNextDay = true;
    }
}
