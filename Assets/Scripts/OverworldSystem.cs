using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Ltg8.Dialogue;
using pt_player_3d.Scripts.Rotation;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ltg8
{

    public class OverworldSystem : MonoBehaviour
    {
        [SerializeField] private TMP_Text deceptionText;
        
        public PathingSystem Pathing { get; private set; }
        public BucketControls BucketControls { get; private set; }
        public BucketItemHolder BucketItemHolder { get; private set; }
        public PlayerDialogueSystem PlayerDialogue { get; private set; }
        public NpcDialogueSystem NpcDialogue { get; private set; }
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
            NpcDialogue = FindAnyObjectByType<NpcDialogueSystem>();
            PlayerDialogue = FindAnyObjectByType<PlayerDialogueSystem>();
            BucketItemHolder = FindAnyObjectByType<BucketItemHolder>();
            BucketControls = FindAnyObjectByType<BucketControls>();
            Pathing = FindAnyObjectByType<PathingSystem>();
            
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
                Game.Save.DailyDeception = 0;
                ReadyForNextDay = false;
                while (Game.Save.TimeOfDay != TimeOfDay.Night && !token.IsCancellationRequested)
                {
                    OverworldEvent e = EventFactory.SpawnEvent();
                    await UniTask.WaitUntil(() => e.IsDone, cancellationToken: token);
                    await GameUtil.Save();
                    Destroy(e.gameObject);
                }
                await UniTask.WaitUntil(() => ReadyForNextDay, cancellationToken: token);
                DayNight.ResetTime();
                
                deceptionText.gameObject.SetActive(true);
                deceptionText.SetText($"You were deceived {Game.Save.DailyDeception} times today.");
                await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: token);
                deceptionText.gameObject.SetActive(false);
            }
        }
    }
}
