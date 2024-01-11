using System;
using pt_player_3d.Scripts.Interaction;

public enum BucketState
{
    AtBottom,
    AtTop,
}

public class BridgeControls : Interactable
{
    
}

public class Bed : Interactable
{
    public override void Interact()
    {
        base.Interact(); 
        Game.Overworld.ReadyForNextDay = true;
    }
}
