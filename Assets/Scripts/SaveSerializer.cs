using System.IO;
using Cysharp.Threading.Tasks;
using poetools.Console.Commands;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections.LowLevel.Unsafe.NotBurstCompatible;
using Unity.Serialization.Binary;
using UnityEngine;

public class SaveSerializer : MonoBehaviour, IConsoleDebugInfo
{
    public static string SavePath => $"{Application.persistentDataPath}/saves";
    public bool Busy { get; private set; }
    
    public async UniTask WriteToDisk(string saveId, SaveBlock data)
    {
        if (Busy) return;
        Busy = true;

        string path = $"{SavePath}/{saveId}.json";
        string dirName = Path.GetDirectoryName(path);
        
        if (!Directory.Exists(dirName) && dirName != null)
            Directory.CreateDirectory(dirName);

        string json = JsonUtility.ToJson(Game.Save, true);
        await File.WriteAllTextAsync(path, json);
        
        await UniTask.Yield();
        Busy = false;
    }

    public async UniTask<SaveBlock> ReadFromDisk(string saveId)
    {
        if (Busy) return null;

        string path = $"{SavePath}/{saveId}.json";

        // not sure if this is best approach for unavailable saves, but its good for now
        if (!File.Exists(path))
            return new SaveBlock();
            
        Busy = true;

        string json = await File.ReadAllTextAsync(path);
        SaveBlock result = JsonUtility.FromJson<SaveBlock>(json);
        
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
        {
            WriteToDisk(_currentDebugSaveId, Game.Save).Forget();
        }
        
        if (GUILayout.Button("Load From Disk"))
            Game.Save = ReadFromDisk(_currentDebugSaveId).GetAwaiter().GetResult();

        GUI.enabled = true;

#if UNITY_EDITOR
        if (GUILayout.Button("Open Save Folder"))
            UnityEditor.EditorUtility.RevealInFinder(SavePath);
#endif
    }
}
