using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
namespace Ltg8
{
    public class DecisionSystem : MonoBehaviour
    {
        public BridgeControls bridgeControls;
        public BucketControls bucketControls;
        
        private Dictionary<string, Func<UniTaskVoid>> _activeChoices;
         
        public void ChoiceLowerBridge(string id, Func<UniTaskVoid> callback)
        {
        }

        public void ChoiceDialogueOption(string id, string text, Func<UniTaskVoid> callback)
        {
        }

        public void ChoiceGiveBucketItem(string id, ItemType item, Func<UniTaskVoid> callback)
        {
        }

        public void RemoveChoice(string id)
        {
        }

        public void RemoveAllChoices()
        {
        }
    }
}
