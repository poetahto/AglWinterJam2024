using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public static class GameUtil
{
    public enum Gender
    {
        Female,
        Male,
    }

    public struct CharacterData
    {
        public string FirstName;
        public string LastName;
        public Gender Gender;
    }
    
    public static CharacterData GetRandomCharacter()
    {
        Gender gender = Random.value > 0.5f ? Gender.Male : Gender.Female;
        CharacterData data = new CharacterData {
            Gender = gender,
            FirstName = gender == Gender.Male ? Names.firstNamesM[Random.Range(0, Names.firstNamesM.Length)] : Names.firstNamesFM[Random.Range(0, Names.firstNamesFM.Length)],
            LastName = Names.lastNames[Random.Range(0, Names.lastNames.Length)],
        };
        return data;
    }
    
    public static bool TryGetItemInBucket(ItemType type, out OverworldItemView item)
    {
        foreach (OverworldItemView itemView in Game.Overworld.BucketItemHolder.Items)
        {
            if (itemView.itemType == type)
            {
                item = itemView;
                return true;
            }
        }

        item = null;
        return false;
    }

    public static async UniTask WaitForBucketToLower()
    {
        while (Game.Save.BucketState != BucketState.AtBottom)
            await UniTask.Yield();
    }

    public static bool TryRemoveItemFromBucket(ItemType type)
    {
        if (TryGetItemInBucket(type, out OverworldItemView item))
        {
            Game.Overworld.Items.DestroyItem(item);
            return true;
        }

        return false;
    }

    public static OverworldItemView PutItemInBucket(ItemType type)
    {
        ItemData itemData = new ItemData {
            type = type,
            position = Game.Overworld.BucketItemHolder.transform.position,
            rotation = Quaternion.identity,
        };
        return Game.Overworld.Items.SpawnItem(itemData);
    }
    
    public static async UniTask Save()
    {
        await Game.Serializer.WriteToDisk("persistent_save", Game.PersistentSave);
        await Game.Serializer.WriteToDisk(Game.PersistentSave.CurrentSaveId, Game.Save);
    }

    public static async UniTask Load()
    {
        Game.PersistentSave = await Game.Serializer.ReadFromDisk("persistent_save", new PersistentSaveData());
        Game.Save = await Game.Serializer.ReadFromDisk(Game.PersistentSave.CurrentSaveId, new OverworldSaveData());
    }

    public static void SayText(this GameObject gameObject, string text, float autoClearSeconds = -1)
    {
        Game.Overworld.NpcDialogue.ShowText(gameObject, text);
        
        if (autoClearSeconds > 0)
            AutoClearText(gameObject, autoClearSeconds).Forget();
    }

    private static async UniTaskVoid AutoClearText(GameObject target, float duration)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        ClearText(target);
    }

    public static void ClearText(this GameObject gameObject)
    {
        Game.Overworld.NpcDialogue.ClearText(gameObject);
    }
}
