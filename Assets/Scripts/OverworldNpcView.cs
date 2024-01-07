using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ltg8
{

    public class OverworldNpcView : MonoBehaviour
    {
        public NpcType type;
        [NonSerialized] public int Id;

        public void Initialize(int id)
        {
            Id = id;
        }
    }

    [Serializable]
    public struct NpcData
    {
        public NpcType type;
        public NpcHatType hat;
        public NpcHeldItemType item;
        public Vector3 position;
        public Vector3 rotation;
    }

    public enum NpcType
    {
        Invalid = 0,
        Normal,
    }
    
    public enum NpcHatType
    {
        None = 0,
        TopHat,
    }

    public enum NpcHeldItemType
    {
        None = 0,
        Duck,
    }

    public class NpcGenerator
    {
        private readonly Dictionary<int, OverworldNpcView> _spawnedNpcViews = new Dictionary<int, OverworldNpcView>();

        public void SpawnSavedNpcs()
        {
            for (int i = 0; i < Game.Save.Npcs.Length; i++)
            {
                if (Game.Save.Npcs[i].type != NpcType.Invalid && !_spawnedNpcViews.ContainsKey(i))
                    CreateNpcWithId(i, Game.Save.Npcs[i]);
            }
        }

        public bool NpcIsSpawned(int saveIndex)
        {
            return _spawnedNpcViews.ContainsKey(saveIndex);
        }

        public OverworldNpcView FindNpc(int saveIndex)
        {
            return _spawnedNpcViews[saveIndex];
        }

        private OverworldNpcView CreateNpcWithId(int id, NpcData data)
        {
            foreach (OverworldNpcView npcView in Game.Settings.npcViewPrefabs)
            {
                if (npcView.type == data.type)
                {
                    OverworldNpcView result = UnityEngine.Object.Instantiate(npcView, data.position, Quaternion.Euler(data.rotation));
                    result.Initialize(id);
                    Game.Save.Npcs[id].type = data.type;
                    _spawnedNpcViews.Add(id, result);
                    return result;
                }
            }

            throw new Exception($"Tried to spawn an item that we don't have a prefab for yet! {data.type.ToString()}");
        }

        public OverworldNpcView SpawnNpc(NpcData data)
        {
            // Find a unused index to store this item in our save.
            int id = -1;
            
            for (int i = 0; i < Game.Save.Npcs.Length; ++i)
            {
                if (Game.Save.Npcs[i].type == NpcType.Invalid)
                {
                    id = i;
                    break;
                }
            }

            if (id == -1)
                throw new Exception("No room left in the save block for items!");

            return CreateNpcWithId(id, data);
        }

        public void DestroyNpc(OverworldNpcView npcView)
        {
            Game.Save.Npcs[npcView.Id].type = NpcType.Invalid;
            _spawnedNpcViews.Remove(npcView.Id);
            UnityEngine.Object.Destroy(npcView.gameObject);
        }
    }
}
