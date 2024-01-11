using Cysharp.Threading.Tasks;
using DefaultNamespace;
using FMODUnity;
using UnityEngine;
using UnityEngine.Splines;

namespace Ltg8
{
    public class TestEvent : OverworldEvent
    {
        [SerializeField] private GameObject npc;
        [SerializeField] private SplineContainer npcIntroSpline;
        [SerializeField] private SplineContainer npcOutroSpline;
        [SerializeField] private FlipBookSpeechAnimator speechAnimator;
        [SerializeField] private EventReference introDialogue;
        [SerializeField] private EventReference responseToOpenBridge;
        [SerializeField] private EventReference responseToAskForId;
        [SerializeField] private EventReference responseToAskForMoney;
        [SerializeField] private EventReference responseToAskForReason;
        [SerializeField] private EventReference responseToGiveDuck;
        
        private bool _isDone;
        public override bool IsDone => _isDone;

        public override void OnStartOverworld()
        {
            base.OnStartOverworld();
            Logic().Forget();
        }

        private async UniTaskVoid Logic()
        {
            // walk up to the bridge.
            await npc.FollowPath(npcIntroSpline);
            
            // declare name, and ask to be let through.
            await speechAnimator.PlaySpeech(introDialogue);
            
            // [ask for id], [ask for money], [ask why], [give duck], [open bridge]
            Decisions.ChoiceLowerBridge("open_bridge", HandleOpenBridge);
            Decisions.ChoiceDialogueOption("ask_for_id", HandleAskForId, "Let's see an ID first.");
            Decisions.ChoiceDialogueOption("ask_for_money", HandleAskForMoney, "No. But you could give me something to change my mind.");
            Decisions.ChoiceDialogueOption("ask_for_reason", HandleAskForReason, "Why should I?");
            Decisions.ChoiceGiveBucketItem("give_duck", HandleGiveDuck, ItemType.ToyDuck);
            Decisions.BeginDecision();
        }

        private async UniTaskVoid HandleOpenBridge()
        {
            await speechAnimator.PlaySpeech(responseToOpenBridge);
            await npc.FollowPath(npcOutroSpline); 
            _isDone = true;
        }
        private async UniTaskVoid HandleAskForId()
        {
            await speechAnimator.PlaySpeech(responseToAskForId);
            _isDone = true;
        }
        private async UniTaskVoid HandleAskForMoney()
        {
            await speechAnimator.PlaySpeech(responseToAskForMoney);
            _isDone = true;
        }
        private async UniTaskVoid HandleAskForReason()
        {
            await speechAnimator.PlaySpeech(responseToAskForReason);
            _isDone = true;
        }
        private async UniTaskVoid HandleGiveDuck()
        {
            await speechAnimator.PlaySpeech(responseToGiveDuck);
            _isDone = true;
        }
    }

}
