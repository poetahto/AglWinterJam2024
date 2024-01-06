using DefaultNamespace;
using UnityEngine;

public static class Game
{
    public static SaveBlock SaveBlock = new SaveBlock();
    public static ItemSystem ItemSystem;
}

public class GameInitializer : MonoBehaviour
{
    private void Awake()
    {
        Game.SaveBlock = new SaveBlock();
        Game.ItemSystem = FindAnyObjectByType<ItemSystem>();
    }
}
