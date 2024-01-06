using DefaultNamespace;
using UnityEngine;

public class OverworldSaveData
{
    // todo: idk dont rlly like constants here
    public const int MaxItems = 100;
    public const int MaxPreSpawnedItems = 100;
    public const int NoHeldItem = -1;

    public bool HasHeldItemTarget;
    public Vector3 HeldItemTarget;
    public int HeldItemIndex = NoHeldItem;
    public ItemData[] Items = new ItemData[MaxItems];
    public string[] PreSpawnedItems = new string[MaxPreSpawnedItems];
    public Time Time;
    public Vector3 PlayerPosition;
    public Vector3 PlayerRotation;
    public string PlayerScene;
}
