using UnityEngine;
namespace DefaultNamespace
{
    public class HeldItemSystem : MonoBehaviour
    {
        public float heldItemSpeed = 15f;
        
        private void Update()
        {
            if (Game.SaveBlock.HasHeldItemTarget && Game.SaveBlock.HeldItemIndex != SaveBlock.NoHeldItem)
            {
                OverworldItemView itemView = Game.ItemSystem.FindItem(Game.SaveBlock.HeldItemIndex);
                itemView.Body.velocity = (Game.SaveBlock.HeldItemTarget - itemView.transform.position) * heldItemSpeed;
            }
        }
    }
}
