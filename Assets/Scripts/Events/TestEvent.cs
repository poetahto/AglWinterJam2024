using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Splines;

namespace Ltg8
{
    public class TestEvent : OverworldEvent
    {
        [SerializeField] private GameObject npc;
        [SerializeField] private SplineContainer npcIntroSpline;
        [SerializeField] private SplineContainer npcOutroSuccess;
        [SerializeField] private SplineContainer npcOutroFailure;
        
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
            npc.SayText("Hello good sir. I am looking for passage into this city.");
            
            Decisions.ChoiceLowerBridge("open_bridge", HandleOpenBridge);
            Decisions.ChoiceDialogueOption("ask_for_id", HandleAskForId, "Let's see an ID first.");
            Decisions.ChoiceDialogueOption("ask_for_money", HandleAskForMoney, "No. But you could give me something to change my mind.");
            Decisions.ChoiceDialogueOption("ask_for_reason", HandleAskForReason, "Why should I?");
            Decisions.ChoiceGiveBucketItem("give_duck", HandleGiveDuck, ItemType.ToyDuck);
            Decisions.BeginDecision();
        }

        private async UniTaskVoid HandleOpenBridge()
        {
            npc.SayText("Thanks!", autoClearSeconds: 3);
            await npc.FollowPath(npcOutroSuccess); 
            _isDone = true;
        }
        private async UniTaskVoid HandleAskForId()
        {
            npc.SayText("Uh, sorry. I don't have one.", autoClearSeconds: 3);
            await npc.FollowPath(npcOutroFailure); 
            _isDone = true;
        }
        private async UniTaskVoid HandleAskForMoney()
        {
            npc.SayText("Ok fine...");
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            npc.SayText("What do you want?");
            Decisions.ChoiceDialogueOption("ask_for_duck", HandleAskForDuck, "I need more ducks for my collection.");
            Decisions.ChoiceDialogueOption("ask_for_soul", HandleAskForSoul, "I need a blood sacrifice.");
            Decisions.BeginDecision();
        }
        private async UniTaskVoid HandleAskForSoul()
        {
            npc.SayText("Yeah... I'm good.");
            await npc.FollowPath(npcOutroFailure);
            _isDone = true;
        }
        private async UniTaskVoid HandleAskForDuck()
        {
            npc.SayText("Oh cool! I actually found one of those earlier.");
            await UniTask.Delay(TimeSpan.FromSeconds(1));

            if (Game.Save.BucketState != BucketState.AtBottom)
            {
                npc.SayText("Send the bucket down and I'll put it in.");
                await GameUtil.WaitForBucketToLower();
            }
            
            GameUtil.PutItemInBucket(ItemType.ToyDuck);
            npc.SayText("I put it in the bucket.");
            
            Decisions.ChoiceLowerBridge("lower_bridge", HandleOpenBridge);
            Decisions.ChoiceDialogueOption("troll", HandleTrolling, "Thanks for the free duck, but you're still not getting in.");
        }
        private async UniTaskVoid HandleTrolling()
        {
            npc.SayText("MAN YOU SUCK!!!");
            await npc.FollowPath(npcOutroFailure);
            _isDone = true;
        }
        private async UniTaskVoid HandleAskForReason()
        {
            npc.SayText("You ask too many questions.", autoClearSeconds: 3);
            await npc.FollowPath(npcOutroFailure); 
            _isDone = true;
        }
        private async UniTaskVoid HandleGiveDuck()
        {
            npc.SayText("Is this a duck?");
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            npc.SayText("Thanks!", autoClearSeconds: 3);
            GameUtil.TryRemoveItemFromBucket(ItemType.ToyDuck);
            await npc.FollowPath(npcOutroFailure); 
            _isDone = true;
        }
    }

}
