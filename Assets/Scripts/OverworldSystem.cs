﻿using Cysharp.Threading.Tasks;
using DefaultNamespace;
using pt_player_3d.Scripts.Rotation;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Ltg8
{
    public class OverworldSystem : MonoBehaviour
    {
        public ItemSystem Items { get; private set; }
        
        public async UniTask LoadFromSave()
        {
            await SceneManager.LoadSceneAsync(Game.Save.PlayerScene, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(Game.Save.PlayerScene));
            Items = FindAnyObjectByType<ItemSystem>();
            
            PlayerSaveTracker playerSaveTracker = FindAnyObjectByType<PlayerSaveTracker>();
            GameObject player = playerSaveTracker == null ? Instantiate(Game.Settings.playerPrefab) : playerSaveTracker.gameObject;
            player.transform.position = Game.Save.PlayerPosition;
            player.GetComponent<RotationSystem>().Rotation = Game.Save.PlayerRotation;
            
            Game.ItemSystem.SpawnSavedItems(); // todo: better handle diff scenes
        }

        public async UniTask LoadScene(string sceneName)
        {
            if (Game.Save.PlayerScene == sceneName)
            {
                await LoadFromSave();
                return;
            }
                
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            Items = FindAnyObjectByType<ItemSystem>();
            Game.ItemSystem.SpawnSavedItems(); // todo: better handle diff scenes
        }
    }
}
