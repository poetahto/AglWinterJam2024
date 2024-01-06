using System.IO;
using Cysharp.Threading.Tasks;
using poetools.Console.Commands;
using UnityEditor.Rendering;
using UnityEngine;

public class SaveSerializer : MonoBehaviour, IConsoleDebugInfo
{
    public static string SavePath => $"{Application.persistentDataPath}/saves";
    public bool Busy { get; private set; }
    
    public async UniTask WriteToDisk<T>(string fileId, T data)
    {
        if (Busy)
        {
            Debug.LogError("Tried to write to the disk while serializer is busy!");
            return;
        }
        Busy = true;

        string path = $"{SavePath}/{fileId}.json";
        string dirName = Path.GetDirectoryName(path);
        
        if (!Directory.Exists(dirName) && dirName != null)
            Directory.CreateDirectory(dirName);

        string json = JsonUtility.ToJson(data, true);
        await File.WriteAllTextAsync(path, json);
        
        await UniTask.Yield();
        Busy = false;
    }

    public async UniTask<T> ReadFromDisk<T>(string fileId, T defaultValue)
    {
        if (Busy)
        {
            Debug.LogError("Tried to read to the disk while serializer is busy!");
            return default;
        }

        string path = $"{SavePath}/{fileId}.json";

        if (!File.Exists(path))
            return defaultValue;
            
        Busy = true;

        string json = await File.ReadAllTextAsync(path);
        T result = JsonUtility.FromJson<T>(json);
        
        Busy = false;
        return result;
    }

    public string DebugName => "serializer";
    public const string DebugSaveId = "dev_test";
    private string _currentDebugSaveId = DebugSaveId;
        
    public void DrawDebugInfo()
    {
        GUILayout.Label("Target Save ID");
        _currentDebugSaveId = GUILayout.TextField(_currentDebugSaveId);
        GUI.enabled = !Busy;

        if (GUILayout.Button("Save To Disk"))
            WriteSaveData().Forget();

        if (GUILayout.Button("Load From Disk"))
            ReadSaveData().Forget();

        GUI.enabled = true;

#if UNITY_EDITOR
        if (GUILayout.Button("Open Save Folder"))
            UnityEditor.EditorUtility.RevealInFinder(SavePath);
#endif
    }
    
    private async UniTaskVoid WriteSaveData()
    {
        await WriteToDisk(_currentDebugSaveId, Game.Save);
        await WriteToDisk("persistent_save", Game.PersistentSave);
    }

    private async UniTaskVoid ReadSaveData()
    {
        Game.Save = await ReadFromDisk(_currentDebugSaveId, new OverworldSaveData());
        Game.PersistentSave = await ReadFromDisk("persistent_save", new PersistentSaveData());
    }
}
