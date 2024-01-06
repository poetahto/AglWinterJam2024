using DefaultNamespace;
using UnityEngine;

public static class Game
{
    public static SaveBlock Save = new SaveBlock();
    public static SaveSerializer Serializer;
    public static ItemSystem ItemSystem;
}

public class GameInitializer : MonoBehaviour
{
    private void Awake()
    {
        Game.Save = new SaveBlock();
        Game.ItemSystem = FindAnyObjectByType<ItemSystem>();
        Game.Serializer = FindAnyObjectByType<SaveSerializer>();
    }
}
