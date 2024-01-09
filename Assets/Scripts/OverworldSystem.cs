using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using pt_player_3d.Scripts.Rotation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ltg8
{
    public class OverworldNpcView : MonoBehaviour
    {
        public NpcType type;
    }

    [CreateAssetMenu]
    public class FlipBookAnimation : ScriptableObject
    {
        
    }

    public class FlipBookMovementAnimator : MonoBehaviour
    {
        [SerializeField] private float walkThreshold = 1;
        [SerializeField] private SpriteRenderer spriteRenderer;
        private Vector3 _prevPos;

        private void Update()
        {
            Vector3 curPos = transform.position;
            Vector3 velocity = (curPos - _prevPos) / Time.deltaTime;
            float speed = velocity.magnitude;
            _prevPos = curPos;
        }
    }

    public enum NpcType
    {
        Farmer,
    }
    
    public class OverworldSystem : MonoBehaviour
    {
        private OverworldEventFactory _eventFactory;

        public ItemSystem Items { get; private set; }
        public DayNightSystem DayNight { get; private set; }
        public bool ReadyForNextDay { get; set; }
        
        public async UniTask LoadFromSave()
        {
            await SceneManager.LoadSceneAsync(Game.Save.PlayerScene, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(Game.Save.PlayerScene));
            Items = FindAnyObjectByType<ItemSystem>();
            DayNight = FindObjectOfType<DayNightSystem>();
            _eventFactory = FindAnyObjectByType<OverworldEventFactory>();
            
            PlayerSaveTracker playerSaveTracker = FindAnyObjectByType<PlayerSaveTracker>();
            GameObject player = playerSaveTracker == null ? Instantiate(Game.Settings.playerPrefab) : playerSaveTracker.gameObject;
            player.transform.position = Game.Save.PlayerPosition;
            player.GetComponent<RotationSystem>().Rotation = Game.Save.PlayerRotation;
            
            Game.ItemSystem.SpawnSavedItems(); // todo: better handle diff scenes
            OverworldGameplayLoop(gameObject.GetCancellationTokenOnDestroy()).Forget(); // todo: this cts probably isnt good
        }

        private async UniTask OverworldGameplayLoop(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                ReadyForNextDay = false;
                while (Game.Save.TimeOfDay != TimeOfDay.Night && !token.IsCancellationRequested)
                {
                    OverworldEvent e = _eventFactory.SpawnEvent();
                    await UniTask.WaitUntil(() => e.IsDone, cancellationToken: token);
                    await GameUtil.Save();
                }
                await UniTask.WaitUntil(() => ReadyForNextDay, cancellationToken: token);
                DayNight.ResetTime();
            }
        }
    }
}
