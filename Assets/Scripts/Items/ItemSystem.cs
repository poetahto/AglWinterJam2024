using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class ItemSystem : MonoBehaviour
    {
        public OverworldItemView[] itemViewPrefabs;

        private readonly Dictionary<int, OverworldItemView> _spawnedItemViews = new Dictionary<int, OverworldItemView>();

        public void SpawnSavedItems()
        {
            for (int i = 0; i < OverworldSaveData.MaxItems; i++)
            {
                if (Game.Save.Items[i].Type != ItemType.Invalid && !_spawnedItemViews.ContainsKey(i))
                    CreateItemAtIndex(i, Game.Save.Items[i]);
            }
        }

        public OverworldItemView FindItem(int saveIndex)
        {
            return _spawnedItemViews[saveIndex];
        }

        private OverworldItemView CreateItemAtIndex(int index, ItemData data)
        {
            foreach (OverworldItemView itemView in itemViewPrefabs)
            {
                if (itemView.itemType == data.Type)
                {
                    OverworldItemView result = Instantiate(itemView, data.Position, data.Rotation);
                    result.Initialize(index);
                    Game.Save.Items[index].Type = data.Type;
                    _spawnedItemViews.Add(index, result);
                    return result;
                }
            }

            throw new Exception($"Tried to spawn an item that we don't have a prefab for yet! {data.Type.ToString()}");
        }

        public OverworldItemView SpawnItem(ItemData data)
        {
            // Find a unused index to store this item in our save.
            int saveIndex = -1;
            
            for (int i = 0; i < OverworldSaveData.MaxItems; ++i)
            {
                if (Game.Save.Items[i].Type == ItemType.Invalid)
                {
                    saveIndex = i;
                    break;
                }
            }

            if (saveIndex == -1)
                throw new Exception("No room left in the save block for items!");

            return CreateItemAtIndex(saveIndex, data);
        }

        public void DestroyItem(OverworldItemView itemView)
        {
            Game.Save.Items[itemView.SaveIndex].Type = ItemType.Invalid;
            _spawnedItemViews.Remove(itemView.SaveIndex);
            Destroy(itemView.gameObject);
        }
    }
}
