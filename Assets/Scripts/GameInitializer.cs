using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Ltg8;
using UnityEngine;

public static class Game
{
    public static GameSettings Settings;
    public static OverworldSaveData Save = new OverworldSaveData();
    public static PersistentSaveData PersistentSave = new PersistentSaveData();
    public static SaveSerializer Serializer;
    public static OverworldSystem Overworld;
}

public class GameInitializer : MonoBehaviour
{
    public GameSettings settings;
    
    public async UniTask Initialize()
    {
        Game.Settings = settings;
        Game.Serializer = FindAnyObjectByType<SaveSerializer>();
        Game.Overworld = FindAnyObjectByType<OverworldSystem>();

        // Load save data from disk.
        Game.PersistentSave = await Game.Serializer.ReadFromDisk("persistent_save", new PersistentSaveData());
        Game.Save = await Game.Serializer.ReadFromDisk(Game.PersistentSave.CurrentSaveId, new OverworldSaveData());
    }
}
