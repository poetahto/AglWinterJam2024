using DefaultNamespace;
using UnityEngine;

public class OverworldSaveData
{
    // todo: idk dont rlly like constants here
    public const int NoHeldItem = -1;

    public bool HasHeldItemTarget;
    public Vector3 HeldItemTarget;
    public int HeldItemIndex = NoHeldItem;
    public ItemData[] Items = new ItemData[100];
    public string[] PreSpawnedItems = new string[100];
    public TimeOfDay TimeOfDay;
    public Vector3 PlayerPosition;
    public Vector3 PlayerRotation;
    public string PlayerScene;
    public BucketState BucketState = BucketState.AtTop;
}
