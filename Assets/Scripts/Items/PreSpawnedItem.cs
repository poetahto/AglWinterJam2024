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

            if (Game.Save.PreSpawnedItems.All(s => s != itemNameId))
            {
                Game.ItemSystem.SpawnItem(itemView.GetItemData());

                // save our name in the pre-spawned items list so we dont get spawned again.
                for (int i = 0; i < Game.Save.PreSpawnedItems.Length; i++)
                {
                    if (Game.Save.PreSpawnedItems[i] == null)
                    {
                        Game.Save.PreSpawnedItems[i] = itemNameId;
                        break;
                    }
                }
            }
            
            // this object is always temporary - item system is the ultimate source of item spawning
            Destroy(gameObject);
        }
    }
}
