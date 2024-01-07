using System.Collections.Generic;
using Ltg8;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    public List<OverworldNpcView> npcViewPrefabs;
    public List<string> overworldSceneNames;
    public GameObject playerPrefab;
}
