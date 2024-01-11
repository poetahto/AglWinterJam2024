using Cysharp.Threading.Tasks;
using Ltg8;
using UnityEngine;
namespace a
{
    public class InjuredPersonEvent : OverworldEvent
    {
        private bool _isFaking;
        
        protected override async UniTask RunLogic()
        {
            _isFaking = Random.value > 0.5f;

            NpcSay(_isFaking ? "Please sir this hurts!" : "AHHH THE PAIN!!!", 0);
            await NpcMoveAll(PathType.HillsToEntrance);
            NpcSay("Please help my friend, he " + (_isFaking ? "got hurt!" : "is gravely injured!"), 1);
            Decisions.ChoiceLowerBridge("a", HandleLowerBridge);
            Decisions.ChoiceDialogueOption("b", HandleHowDidHeGetHurt, "How did he get hurt?");
            Decisions.ChoiceDialogueOption("c", HandlePresentId, "Please present your ID.");
            Decisions.Initialize();
        }
        private async UniTaskVoid HandlePresentId()
        {
            if (_isFaking) 
                NpcSay("Please officer there is not time for this we need to get in!", 0);
            
            else NpcSay("There is no time! He will die if he does not get help now!", 1);
            
            Decisions.ChoiceDialogueOption("a", HandleReject, "Sorry, I don't believe you.");
            Decisions.ChoiceLowerBridge("b", HandleLowerBridge);
            Decisions.Initialize();
        }
        private async UniTaskVoid HandleHowDidHeGetHurt()
        {
            NpcSay("It was a hunting accident, I shot him by accident", 1);
            
            if (_isFaking)
                NpcSay("I need to see the doctor, officer.", 0);
            
            Decisions.ChoiceDialogueOption("a", HandleReject, "Sorry, I don't believe you.");
            Decisions.ChoiceLowerBridge("b", HandleLowerBridge);
            Decisions.Initialize();
        }
        private async UniTaskVoid HandleLowerBridge()
        {
            FinishEvent();
            await NpcMoveAll(PathType.EntranceToCity);
            Game.Save.Karma += _isFaking ? -1 : 1;
            if (_isFaking) Game.Save.DailyDeception++;
        }
        private async UniTaskVoid HandleReject()
        {
            FinishEvent();
            NpcSay(!_isFaking ? "How dare you doom him to death!" : "Rats.", 1);
            await NpcMoveAll(PathType.EntranceToHills);
            Game.Save.Karma += _isFaking ? 1 : -1;
        }
    }
}
