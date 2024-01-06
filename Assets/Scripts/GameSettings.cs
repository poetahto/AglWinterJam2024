using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    public List<string> overworldSceneNames;
    public GameObject playerPrefab;
}
