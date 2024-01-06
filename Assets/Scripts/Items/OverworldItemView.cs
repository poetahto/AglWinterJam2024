using System;
using FMODUnity;
using pt_player_3d.Scripts.Interaction;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Rigidbody))]
    public class OverworldItemView : Interactable
    {
        public ItemType itemType;
        public EventReference interactSound;
        
        [NonSerialized]
        public Rigidbody Body;
        
        [NonSerialized]
        public int SaveIndex;

        private void Awake()
        {
            Body = GetComponent<Rigidbody>();
        }

        public void Initialize(int saveIndex)
        {
            SaveIndex = saveIndex;

            if (Game.SaveBlock.HeldItemIndex == SaveIndex)
                SetHeld(true);
        }

        private void Update()
        {
            Transform t = transform;
            Game.SaveBlock.Items[SaveIndex].Position = t.position;
            Game.SaveBlock.Items[SaveIndex].Rotation = t.rotation;
        }

        public void SetHeld(bool isHeld)
        {
            Body.useGravity = !isHeld;
        }

        public override void Interact()
        {
            base.Interact();
            RuntimeManager.PlayOneShot(interactSound);

            if (Game.SaveBlock.HeldItemIndex == SaveIndex)
            {
                SetHeld(false);
                Game.SaveBlock.HeldItemIndex = SaveBlock.NoHeldItem;
                return;
            }
            
            if (Game.SaveBlock.HeldItemIndex != SaveBlock.NoHeldItem)
                Game.ItemSystem.FindItem(Game.SaveBlock.HeldItemIndex).SetHeld(false);
            
            Game.SaveBlock.HeldItemIndex = SaveIndex;
            SetHeld(true);
        }

        public ItemData GetItemData()
        {
            Transform t = transform;
            
            return new ItemData {
                Position = t.position,
                Rotation = t.rotation,
                Type = itemType,
            };
        }
    }
}
