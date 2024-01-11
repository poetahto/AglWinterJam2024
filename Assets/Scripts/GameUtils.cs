using Cysharp.Threading.Tasks;

public static class GameUtil
{
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
