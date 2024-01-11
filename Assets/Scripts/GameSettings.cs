using System.Collections.Generic;
using DefaultNamespace;
using Ltg8;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    public List<OverworldItemView> itemPrefabs;
    public List<string> overworldSceneNames;
    public OverworldPlayerView playerPrefab;
    public KeyCode openDialogueKey = KeyCode.E;
}

