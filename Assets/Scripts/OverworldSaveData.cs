using DefaultNamespace;
using UnityEngine;

public class OverworldSaveData
{
    // todo: idk dont rlly like constants here
    public const int InvalidId = -1;

    public int DailyDeception;
    public int Karma;
    public bool HasHeldItemTarget;
    public Vector3 HeldItemTarget;
    public int HeldItemIndex = InvalidId;
    public ItemData[] Items = new ItemData[100];
    public string[] PreSpawnedItems = new string[100];
    public int[] BucketItemIds = new int[10];
    public TimeOfDay TimeOfDay;
    public Vector3 PlayerPosition;
    public Vector3 PlayerRotation;
    public string PlayerScene = "Walls 1";
    public BucketState BucketState = BucketState.AtTop;
}
