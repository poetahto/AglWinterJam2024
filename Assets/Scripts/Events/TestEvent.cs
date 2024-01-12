using Cysharp.Threading.Tasks;
using DefaultNamespace;

namespace Ltg8
{
    public class TestEvent : OverworldEvent
    {
        protected override async UniTask RunLogic()
        {
            await NpcMove(PathType.HillsToEntrance, 0);
            NpcSay("Hello good sir. I am looking for passage into this city.", 0);
            Decisions.ChoiceLowerBridge("open_bridge", HandleOpenBridge);
            Decisions.ChoiceDialogueOption("ask_for_id", HandleAskForId, "Let's see an ID first.");
            Decisions.ChoiceDialogueOption("ask_for_money", HandleAskForMoney, "No. But you could give me something to change my mind.");
            Decisions.ChoiceDialogueOption("ask_for_reason", HandleAskForReason, "Why should I?");
            Decisions.ChoiceGiveBucketItem("give_duck", HandleGiveDuck, ItemType.ToyDuck);
            Decisions.Initialize();
        }
        private async UniTaskVoid HandleOpenBridge()
        {
            NpcSay("Thanks!", 0, autoClearSeconds: 3);
            await NpcMove(PathType.EntranceToCity, 0);
            FinishEvent();
        }
        private async UniTaskVoid HandleAskForId()
        {
            NpcSay("Uh, sorry. I don't have one.", 0, autoClearSeconds: 3);
            await NpcMove(PathType.EntranceToHills, 0);
            FinishEvent();
        }
        private async UniTaskVoid HandleAskForMoney()
        {
            NpcSay("Ok fine...", 0);
            await Delay(1);
            NpcSay("What do you want?", 0);
            Decisions.ChoiceDialogueOption("ask_for_duck", HandleAskForDuck, "I need more ducks for my collection.");
            Decisions.ChoiceDialogueOption("ask_for_soul", HandleAskForSoul, "I need a blood sacrifice.");
            Decisions.Initialize();
        }
        private async UniTaskVoid HandleAskForSoul()
        {
            FinishEvent();
            NpcSay("Yeah... I'm good.", 0, autoClearSeconds:3);
            await NpcMove(PathType.EntranceToHills, 0);
        }
        private async UniTaskVoid HandleAskForDuck()
        {
            NpcSay("Oh cool! I actually found one of those earlier.", 0);
            await Delay(1);
            await NpcWaitForBucket();
            GameUtil.PutItemInBucket(ItemType.ToyDuck);
            NpcSay("I put it in the bucket.", 0);
            
            Decisions.ChoiceLowerBridge("lower_bridge", HandleOpenBridge);
            Decisions.ChoiceDialogueOption("troll", HandleTrolling, "Thanks for the free duck, but you're still not getting in.");
            Decisions.Initialize();
        }
        private async UniTaskVoid HandleTrolling()
        {
            FinishEvent();
            NpcSay("MAN YOU SUCK!!!", 0, autoClearSeconds:3 );
            await NpcMove(PathType.EntranceToHills, 0);
        }
        private async UniTaskVoid HandleAskForReason()
        {
            FinishEvent();
            NpcSay("You ask too many questions.", 0, autoClearSeconds: 3);
            await NpcMove(PathType.EntranceToHills, 0);
        }
        private async UniTaskVoid HandleGiveDuck()
        {
            FinishEvent();
            NpcSay("Is this a duck?", 0);
            await Delay(1);
            NpcSay("Thanks!", 0, autoClearSeconds: 3);
            GameUtil.TryRemoveItemFromBucket(ItemType.ToyDuck);
            await NpcMove(PathType.EntranceToHills, 0);
        }
    }
}
