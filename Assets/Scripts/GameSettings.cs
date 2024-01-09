using System.Collections.Generic;
using Ltg8;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    public List<OverworldNpcView> npcPrefabs;
    public List<string> overworldSceneNames;
    public GameObject playerPrefab;
}

