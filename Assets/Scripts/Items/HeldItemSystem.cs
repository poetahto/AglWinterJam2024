using UnityEngine;
namespace DefaultNamespace
{
    public class HeldItemSystem : MonoBehaviour
    {
        public float heldItemSpeed = 15f;
        
        private void Update()
        {
            if (Game.Save.HasHeldItemTarget && Game.Save.HeldItemIndex != OverworldSaveData.NoHeldItem && Game.ItemSystem.ItemIsSpawned(Game.Save.HeldItemIndex))
            {
                OverworldItemView itemView = Game.ItemSystem.FindItem(Game.Save.HeldItemIndex);
                itemView.Body.velocity = (Game.Save.HeldItemTarget - itemView.transform.position) * heldItemSpeed;
            }
        }
    }
}
