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
        private Func<UniTaskVoid> _selectedCallback;

        protected override void Start()
        {
            base.Start();
            bucketControls.OnBucketStateChange += HandleBucketStateChange;
            bridgeControls.OnBridgeOpenStart += HandleBridgeOpenStart;
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

        public void BeginDecision()
        {
            HandleBucketStateChange(Game.Save.BucketState);
        }

        public void FinishDecision()
        {
            _selectedCallback?.Invoke();
            _selectedCallback = null;
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

            _selectedCallback = _activeChoices[choiceId];
            RemoveAllChoices();
        }

        private void HandleBridgeOpenStart()
        {
            HandleChoiceSelected(_bridgeChoiceId);
            bridgeControls.OnBridgeOpenEnd += HandleBridgeOpenEnd;
        }

        private void HandleBridgeOpenEnd()
        {
            bridgeControls.OnBridgeOpenEnd -= HandleBridgeOpenEnd;
            FinishDecision();
        }

        private void HandleDialogueOptionSelected(string optionId)
        {
            HandleChoiceSelected(optionId);
            FinishDecision();
        }

        private void HandleBucketStateChange(BucketState state)
        {
            if (state == BucketState.AtBottom)
            {
                foreach (OverworldItemView item in bucketItemHolder.Items)
                {
                    if (_bucketChoiceIds.TryGetValue(item.itemType, out string choiceId))
                    {
                        HandleChoiceSelected(choiceId);
                        FinishDecision();
                        break;
                    }
                }
            }
        }
    }
}
