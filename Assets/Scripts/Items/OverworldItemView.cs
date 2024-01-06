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

            if (Game.Save.HeldItemIndex == SaveIndex)
                SetHeld(true);
        }

        private void Update()
        {
            Transform t = transform;
            Game.Save.Items[SaveIndex].Position = t.position;
            Game.Save.Items[SaveIndex].Rotation = t.rotation;
        }

        public void SetHeld(bool isHeld)
        {
            Body.useGravity = !isHeld;
        }

        public override void Interact()
        {
            base.Interact();
            RuntimeManager.PlayOneShot(interactSound);

            if (Game.Save.HeldItemIndex == SaveIndex)
            {
                SetHeld(false);
                Game.Save.HeldItemIndex = SaveBlock.NoHeldItem;
                return;
            }
            
            if (Game.Save.HeldItemIndex != SaveBlock.NoHeldItem)
                Game.ItemSystem.FindItem(Game.Save.HeldItemIndex).SetHeld(false);
            
            Game.Save.HeldItemIndex = SaveIndex;
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
