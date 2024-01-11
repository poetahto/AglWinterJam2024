using Cysharp.Threading.Tasks;
using DefaultNamespace;

namespace Ltg8
{
    public class TestEvent : OverworldEvent
    {
        protected override async UniTask RunLogic()
        {
            await NpcMove(0, PathType.HillsToEntrance);
            NpcSay(0, "Hello good sir. I am looking for passage into this city.");
            Decisions.ChoiceLowerBridge("open_bridge", HandleOpenBridge);
            Decisions.ChoiceDialogueOption("ask_for_id", HandleAskForId, "Let's see an ID first.");
            Decisions.ChoiceDialogueOption("ask_for_money", HandleAskForMoney, "No. But you could give me something to change my mind.");
            Decisions.ChoiceDialogueOption("ask_for_reason", HandleAskForReason, "Why should I?");
            Decisions.ChoiceGiveBucketItem("give_duck", HandleGiveDuck, ItemType.ToyDuck);
            Decisions.Initialize();
        }
        private async UniTaskVoid HandleOpenBridge()
        {
            NpcSay(0, "Thanks!", autoClearSeconds: 3);
            await NpcMove(0, PathType.EntranceToCity);
            FinishEvent();
        }
        private async UniTaskVoid HandleAskForId()
        {
            NpcSay(0, "Uh, sorry. I don't have one.", autoClearSeconds: 3);
            await NpcMove(0, PathType.EntranceToHills);
            FinishEvent();
        }
        private async UniTaskVoid HandleAskForMoney()
        {
            NpcSay(0, "Ok fine...");
            await Delay(1);
            NpcSay(0, "What do you want?");
            Decisions.ChoiceDialogueOption("ask_for_duck", HandleAskForDuck, "I need more ducks for my collection.");
            Decisions.ChoiceDialogueOption("ask_for_soul", HandleAskForSoul, "I need a blood sacrifice.");
            Decisions.Initialize();
        }
        private async UniTaskVoid HandleAskForSoul()
        {
            NpcSay(0, "Yeah... I'm good.");
            await NpcMove(0, PathType.EntranceToHills);
            FinishEvent();
        }
        private async UniTaskVoid HandleAskForDuck()
        {
            NpcSay(0, "Oh cool! I actually found one of those earlier.");
            await Delay(1);

            if (Game.Save.BucketState != BucketState.AtBottom)
            {
                NpcSay(0, "Send the bucket down and I'll put it in.");
                await GameUtil.WaitForBucketToLower();
            }
            
            GameUtil.PutItemInBucket(ItemType.ToyDuck);
            NpcSay(0, "I put it in the bucket.");
            
            Decisions.ChoiceLowerBridge("lower_bridge", HandleOpenBridge);
            Decisions.ChoiceDialogueOption("troll", HandleTrolling, "Thanks for the free duck, but you're still not getting in.");
        }
        private async UniTaskVoid HandleTrolling()
        {
            NpcSay(0, "MAN YOU SUCK!!!");
            await NpcMove(0, PathType.EntranceToHills);
            FinishEvent();
        }
        private async UniTaskVoid HandleAskForReason()
        {
            NpcSay(0, "You ask too many questions.", autoClearSeconds: 3);
            await NpcMove(0, PathType.EntranceToHills);
            FinishEvent();
        }
        private async UniTaskVoid HandleGiveDuck()
        {
            NpcSay(0, "Is this a duck?");
            await Delay(1);
            NpcSay(0, "Thanks!", autoClearSeconds: 3);
            GameUtil.TryRemoveItemFromBucket(ItemType.ToyDuck);
            await NpcMove(0, PathType.EntranceToHills);
            FinishEvent();
        }
    }
}
