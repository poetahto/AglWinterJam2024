using System.Linq;
using UnityEngine;
namespace DefaultNamespace
{
    [RequireComponent(typeof(OverworldItemView))]
    public class PreSpawnedItem : MonoBehaviour
    {
        public string itemNameId;
        
        private void Start()
        {
            OverworldItemView itemView = GetComponent<OverworldItemView>();

            if (Game.SaveBlock.PreSpawnedItems.All(s => s != itemNameId))
            {
                Game.ItemSystem.SpawnItem(itemView.GetItemData());

                // save our name in the pre-spawned items list so we dont get spawned again.
                for (int i = 0; i < Game.SaveBlock.PreSpawnedItems.Length; i++)
                {
                    if (Game.SaveBlock.PreSpawnedItems[i] == string.Empty)
                        Game.SaveBlock.PreSpawnedItems[i] = itemNameId;
                }
            }
            
            // this object is always temporary - item system is the ultimate source of item spawning
            Destroy(gameObject);
        }
    }
}
