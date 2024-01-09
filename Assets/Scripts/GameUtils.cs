using System;
using Cysharp.Threading.Tasks;
using Ltg8;

public static class GameUtil
{
    public static OverworldNpcView FindNpcPrefab(NpcType type)
    {
        foreach (OverworldNpcView npcView in Game.Settings.npcPrefabs)
        {
            if (npcView.type == type)
                return npcView;
        }

        throw new Exception($"No prefab defined for npc {type}");
    }
    
    public static async UniTask Save()
    {
        await Game.Serializer.WriteToDisk("persistent_save", Game.PersistentSave);
        await Game.Serializer.WriteToDisk(Game.PersistentSave.CurrentSaveId, Game.Save);
    }

    public static async UniTask Load()
    {
        Game.PersistentSave = await Game.Serializer.ReadFromDisk("persistent_save", new PersistentSaveData());
        Game.Save = await Game.Serializer.ReadFromDisk(Game.PersistentSave.CurrentSaveId, new OverworldSaveData());
    }
}
