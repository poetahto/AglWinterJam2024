using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using pt_player_3d.Scripts.Rotation;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Ltg8
{
    public class OverworldSystem : MonoBehaviour
    {
        private OverworldNpcSpawner _spawner;

        public NpcGenerator NpcGenerator { get; private set; }
        public ItemSystem Items { get; private set; }
        public DayNightSystem DayNight { get; private set; }
        public bool ReadyForNextDay { get; set; }
        
        public async UniTask LoadFromSave()
        {
            await SceneManager.LoadSceneAsync(Game.Save.PlayerScene, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(Game.Save.PlayerScene));
            Items = FindAnyObjectByType<ItemSystem>();
            DayNight = FindObjectOfType<DayNightSystem>();
            NpcGenerator = new NpcGenerator();
            _spawner = FindAnyObjectByType<OverworldNpcSpawner>();
            
            PlayerSaveTracker playerSaveTracker = FindAnyObjectByType<PlayerSaveTracker>();
            GameObject player = playerSaveTracker == null ? Instantiate(Game.Settings.playerPrefab) : playerSaveTracker.gameObject;
            player.transform.position = Game.Save.PlayerPosition;
            player.GetComponent<RotationSystem>().Rotation = Game.Save.PlayerRotation;
            
            Game.ItemSystem.SpawnSavedItems(); // todo: better handle diff scenes
            OverworldGameplayLoop(gameObject.GetCancellationTokenOnDestroy()).Forget(); // todo: this cts probably isnt good
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
            DayNight = FindObjectOfType<DayNightSystem>();
            NpcGenerator = new NpcGenerator();
            _spawner = FindAnyObjectByType<OverworldNpcSpawner>();
            
            Game.ItemSystem.SpawnSavedItems(); // todo: better handle diff scenes
            OverworldGameplayLoop(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask OverworldGameplayLoop(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                ReadyForNextDay = false;
                _spawner.IsSpawning = true;
                await UniTask.WaitUntil(() => Game.Save.TimeOfDay == TimeOfDay.Night, cancellationToken: token);
                _spawner.IsSpawning = false;
                await UniTask.WaitUntil(() => ReadyForNextDay, cancellationToken: token);
                DayNight.ResetTime();
            }
        }
    }
}
