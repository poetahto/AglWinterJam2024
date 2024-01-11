using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace;

namespace Ltg8
{
    public class DecisionSystem : OverworldBehavior
    {
        public BridgeControls bridgeControls;
        public BucketControls bucketControls;
        public BucketItemHolder bucketItemHolder;
        public DialogueSystem dialogueSystem;
        
        private readonly Dictionary<string, Func<UniTaskVoid>> _activeChoices = new Dictionary<string, Func<UniTaskVoid>>();
        private readonly Dictionary<ItemType, string> _bucketChoiceIds = new Dictionary<ItemType, string>();
        private string _bridgeChoiceId;

        protected override void Start()
        {
            base.Start();
            bucketControls.OnBucketStateChange += HandleBucketStateChange;
            bridgeControls.OnBridgeOpened += HandleBridgeOpened;
            dialogueSystem.OnOptionSelected += HandleDialogueOptionSelected;
        }

        public void ChoiceLowerBridge(string id, Func<UniTaskVoid> callback)
        {
            bridgeControls.IsLocked = false;
            _activeChoices.Add(id, callback);
            _bridgeChoiceId = id;
        }

        public void ChoiceDialogueOption(string id, Func<UniTaskVoid> callback, string text)
        {
            _activeChoices.Add(id, callback);
            dialogueSystem.AddOption(id, text);
        }

        public void ChoiceGiveBucketItem(string id, Func<UniTaskVoid> callback, ItemType item)
        {
            _activeChoices.Add(id, callback);
            _bucketChoiceIds.Add(item, id);
        }

        public void Finish()
        {
            HandleBucketStateChange(Game.Save.BucketState);
        }

        public void RemoveAllChoices()
        {
            _activeChoices.Clear();
            _bucketChoiceIds.Clear();
            _bridgeChoiceId = null;
            dialogueSystem.RemoveAllOptions();
        }

        private void HandleChoiceSelected(string choiceId)
        {
            if (!_activeChoices.ContainsKey(choiceId))
                return;

            _activeChoices[choiceId].Invoke().Forget();
            RemoveAllChoices();
        }

        private void HandleBridgeOpened()
        {
            if (_bridgeChoiceId != null)
                HandleChoiceSelected(_bridgeChoiceId);
        }

        private void HandleDialogueOptionSelected(string optionId)
        {
            HandleChoiceSelected(optionId);
        }

        private void HandleBucketStateChange(BucketState state)
        {
            if (state == BucketState.AtBottom)
            {
                foreach (OverworldItemView item in bucketItemHolder.Items)
                {
                    if (_bucketChoiceIds.TryGetValue(item.itemType, out string choiceId))
                        HandleChoiceSelected(choiceId);
                }
            }
        }
    }
}
