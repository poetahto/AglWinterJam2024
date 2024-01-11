using Cysharp.Threading.Tasks;
using DefaultNamespace;
using TMPro;
using UnityEngine;

namespace Ltg8
{
    public class GaurdEvent : OverworldEvent
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
            NpcSay("*whistling*");
            await Delay(1);
            NpcSay("Aye sir, returning from patrol. Please lower the gates.");
            Decisions.ChoiceDialogueOption("a", HandleAskForId, "I have to check your ID first.");
            Decisions.ChoiceLowerBridge("b", HandleOpenBridge);
            Decisions.ChoiceDialogueOption("c", HandleReject, "Nope, go back on patrol.");
            Decisions.ChoiceDialogueOption("d", HandleAskForName, "What is your name, gaurd?");
            Decisions.Initialize();
        }
        private async UniTaskVoid HandleAskForId()
        {
            if (_isThief)
            {
                NpcSay("Comon mate you know me!");
                Decisions.ChoiceDialogueOption("a", HandleReject, "Sorry, I can't let you in.");
                Decisions.ChoiceLowerBridge("c", HandleOpenBridge);
                Decisions.Initialize();
            } else
            {
                await NpcWaitForBucket();
                NpcSay("There you go.");
                OverworldItemView item = GameUtil.PutItemInBucket(ItemType.IDCard);
                item.GetComponentInChildren<TextMeshPro>().SetText($"{StatedIdentity.FirstName} {StatedIdentity.LastName}");
                Decisions.ChoiceDialogueOption("a", HandleReject, "Sorry, I can't let you in.");
                Decisions.ChoiceLowerBridge("c", HandleOpenBridge);
                Decisions.Initialize();
            }
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
            if (_isThief)
            {
                NpcSay("You've been more of a help than you'll ever know.");
            } else
            {
                NpcSay("Thank you mate!");
            }
            Game.Save.Karma += _isThief ? -2 : 2;
            Game.Save.DailyDeception += _isThief ? 1 : 0;
            await NpcMove(PathType.EntranceToCity);
            FinishEvent();
        }
        private async UniTaskVoid HandleReject()
        {
            if (_isThief)
            {
                NpcSay("This is a bloody outrage mate.");
            }
            else
            {
                NpcSay("What? High command will know about this.");
            }
            await NpcMove(PathType.EntranceToHills);
            Game.Save.Karma += _isThief ? 2 : -2;
            Game.Save.DailyDeception += _isThief ? 1 : 0;
            FinishEvent();
        }
    }
}
