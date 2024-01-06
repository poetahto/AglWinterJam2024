using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class ItemSystem : MonoBehaviour
    {
        public OverworldItemView[] itemViewPrefabs;
        public bool spawnSavedItemsOnStart;

        private readonly Dictionary<int, OverworldItemView> _spawnedItemViews = new Dictionary<int, OverworldItemView>();

        private void Start()
        {
            if (spawnSavedItemsOnStart)
                SpawnSavedItems();
        }

        public void SpawnSavedItems()
        {
            for (int i = 0; i < SaveBlock.MaxItems; i++)
            {
                if (Game.SaveBlock.Items[i].Type != ItemType.Invalid && !_spawnedItemViews.ContainsKey(i))
                    SpawnItem(Game.SaveBlock.Items[i]);
            }
        }

        public OverworldItemView FindItem(int saveIndex)
        {
            return _spawnedItemViews[saveIndex];
        }

        public OverworldItemView SpawnItem(ItemData data)
        {
            // Find a unused index to store this item in our save.
            int saveIndex = -1;
            
            for (int i = 0; i < SaveBlock.MaxItems; ++i)
            {
                if (Game.SaveBlock.Items[i].Type == ItemType.Invalid)
                {
                    saveIndex = i;
                    break;
                }
            }

            if (saveIndex == -1)
                throw new Exception("No room left in the save block for items!");

            // Spawn the prefab that represents this item.
            foreach (OverworldItemView itemView in itemViewPrefabs)
            {
                if (itemView.itemType == data.Type)
                {
                    OverworldItemView result = Instantiate(itemView, data.Position, data.Rotation);
                    result.Initialize(saveIndex);
                    Game.SaveBlock.Items[saveIndex].Type = data.Type;
                    _spawnedItemViews.Add(saveIndex, result);
                    return result;
                }
            }

            throw new Exception($"Tried to spawn an item that we don't have a prefab for yet! {data.Type.ToString()}");
        }

        public void DestroyItem(OverworldItemView itemView)
        {
            Game.SaveBlock.Items[itemView.SaveIndex].Type = ItemType.Invalid;
            _spawnedItemViews.Remove(itemView.SaveIndex);
            Destroy(itemView.gameObject);
        }
    }
}
