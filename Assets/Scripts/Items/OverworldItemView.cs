﻿using System;
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
            Game.Save.Items[SaveIndex].position = t.position;
            Game.Save.Items[SaveIndex].rotation = t.rotation;
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
                Game.Save.HeldItemIndex = OverworldSaveData.NoHeldItem;
                return;
            }
            
            if (Game.Save.HeldItemIndex != OverworldSaveData.NoHeldItem)
                Game.ItemSystem.FindItem(Game.Save.HeldItemIndex).SetHeld(false);
            
            Game.Save.HeldItemIndex = SaveIndex;
            SetHeld(true);
        }

        public ItemData GetItemData()
        {
            Transform t = transform;
            
            return new ItemData {
                position = t.position,
                rotation = t.rotation,
                type = itemType,
            };
        }
    }
}
