using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using pt_player_3d.Scripts.Rotation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Ltg8
{
    public class OverworldSystem : MonoBehaviour
    {
        public OverworldEventFactory EventFactory { get; private set; }
        public DecisionSystem Decisions { get; private set; }
        public ItemSystem Items { get; private set; }
        public DayNightSystem DayNight { get; private set; }
        public OverworldPlayerView Player { get; private set; }
        public bool ReadyForNextDay { get; set; }
        public bool IsLoaded { get; private set; }
        
        public async UniTask LoadFromSave()
        {
            await SceneManager.LoadSceneAsync(Game.Save.PlayerScene, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(Game.Save.PlayerScene));
            await UniTask.Yield();
            Items = FindAnyObjectByType<ItemSystem>();
            DayNight = FindAnyObjectByType<DayNightSystem>();
            Decisions = FindAnyObjectByType<DecisionSystem>();
            EventFactory = FindAnyObjectByType<OverworldEventFactory>();
            Player = FindAnyObjectByType<OverworldPlayerView>();
            
            if (Player == null)
            {
                Player = Instantiate(Game.Settings.playerPrefab);
                Player.transform.position = Game.Save.PlayerPosition;
            }
            
            Player.GetComponent<RotationSystem>().Rotation = Game.Save.PlayerRotation;
            Items.SpawnSavedItems(); // todo: better handle diff scenes

            foreach (OverworldBehavior overworldBehavior in FindObjectsByType<OverworldBehavior>(FindObjectsSortMode.None))
                overworldBehavior.OnStartOverworld();
            
            OverworldGameplayLoop(gameObject.GetCancellationTokenOnDestroy()).Forget(); // todo: this cts probably isnt good
            IsLoaded = true;
        }

        private async UniTask OverworldGameplayLoop(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                ReadyForNextDay = false;
                while (Game.Save.TimeOfDay != TimeOfDay.Night && !token.IsCancellationRequested)
                {
                    OverworldEvent e = EventFactory.SpawnEvent();
                    await UniTask.WaitUntil(() => e.IsDone, cancellationToken: token);
                    await GameUtil.Save();
                }
                await UniTask.WaitUntil(() => ReadyForNextDay, cancellationToken: token);
                DayNight.ResetTime();
            }
        }
    }
}
