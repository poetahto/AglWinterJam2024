using Cysharp.Threading.Tasks;
using DefaultNamespace;
using TMPro;
using UnityEngine;

namespace Ltg8
{
    public class IdPersonEvent : OverworldEvent
    {
        private GameUtil.CharacterData _realIdentity;
        private GameUtil.CharacterData _fakeIdentity;
        private bool _isThief;

        private GameUtil.CharacterData StatedIdentity => _isThief ? _fakeIdentity : _realIdentity;
        
        protected override async UniTask RunLogic()
        {
            _isThief = Random.value > 0.5f;
            _realIdentity = GameUtil.GetRandomCharacter();
            _fakeIdentity = GameUtil.GetRandomCharacter();

            await NpcMove(PathType.HillsToEntrance);
            NpcSay("It's been a long road to get here.");
            await Delay(1);
            NpcSay("Could you let me in to the city?");
            Decisions.ChoiceDialogueOption("a", HandleAskForId, "I have to check your ID first.");
            Decisions.ChoiceLowerBridge("b", HandleOpenBridge);
            Decisions.ChoiceDialogueOption("c", HandleReject, "Nope, go away.");
            Decisions.ChoiceDialogueOption("d", HandleAskForName, "What is your name, traveler?");
            Decisions.Initialize();
        }
        private async UniTaskVoid HandleAskForId()
        {
            await NpcWaitForBucket();
            NpcSay("There you go.");
            OverworldItemView item = GameUtil.PutItemInBucket(ItemType.IDCard);
            item.GetComponentInChildren<TextMeshPro>().SetText($"{StatedIdentity.FirstName} {StatedIdentity.LastName}");
            Decisions.ChoiceDialogueOption("a", HandleReject, "Sorry, I can't let you in.");
            Decisions.ChoiceLowerBridge("c", HandleOpenBridge);
            Decisions.Initialize();
        }
        private async UniTaskVoid HandleAskForName()
        {
            NpcSay($"My name is {_realIdentity.FirstName}.");
            Decisions.ChoiceDialogueOption("a", HandleAskForId, "Ok, now lets see your ID.");
            Decisions.ChoiceLowerBridge("b", HandleOpenBridge);
            Decisions.ChoiceDialogueOption("c", HandleReject, "Nope, go away.");
            Decisions.Initialize();
        }
        private async UniTaskVoid HandleOpenBridge()
        {
            Game.Save.Karma += _isThief ? -1 : 1;
            Game.Save.DailyDeception += _isThief ? -1 : 1;
            await NpcMove(PathType.EntranceToCity);
            FinishEvent();
        }
        private async UniTaskVoid HandleReject()
        {
            await NpcMove(PathType.EntranceToHills);
            Game.Save.Karma += _isThief ? 1 : -1;
            Game.Save.DailyDeception += _isThief ? 1 : -1;
            FinishEvent();
        }
    }
}
