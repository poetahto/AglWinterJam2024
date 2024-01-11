using System;
using System.Collections.Generic;
using Core;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Assertions;

namespace pt_player_3d.Scripts.Interaction
{
    public class CustomInteractionSystem : MonoBehaviour, IInteractionSystem
    {
        private const int BufferSize = 50;

        [SerializeField]
        private Transform viewDirection;

        public bool autoUpdate = true;

        public bool IsInteractHeld { get; set; }

        private bool _wasInteractHeld;
        private RaycastHit[] _hitBuffer = new RaycastHit[BufferSize];
        private Comparer<RaycastHit> _hitDistanceComparer = Comparer<RaycastHit>.Create((hitA, hitB) => hitA.distance.CompareTo(hitB.distance));

        private void FixedUpdate()
        {
            if (autoUpdate)
                Tick(Time.deltaTime);
        }

        public void Tick(float deltaTime)
        {
            if (!_wasInteractHeld && IsInteractHeld) // Just pressed interact
                TryToInteract();

            _wasInteractHeld = IsInteractHeld;
        }

        private void TryToInteract()
        {
            if (Game.Save.HeldItemIndex != OverworldSaveData.InvalidId)
            {
                OverworldItemView heldItem = Game.Overworld.Items.FindItem(Game.Save.HeldItemIndex);
                heldItem.SetHeld(false);
                Game.Save.HeldItemIndex = OverworldSaveData.InvalidId;
                return;
            }
            
            Ray ray = new Ray(viewDirection.transform.position, viewDirection.forward);
            int hits = Physics.RaycastNonAlloc(ray, _hitBuffer, float.PositiveInfinity, Physics.DefaultRaycastLayers);
            Assert.IsTrue(hits <= BufferSize);
            Array.Sort(_hitBuffer, 0, hits, _hitDistanceComparer);

            for (int i = 0; i < hits; i++)
            {
                RaycastHit hit = _hitBuffer[i];

                // if we hit something we can interact with, we can done.
                if (hit.collider.TryGetComponentWithRigidbody(out Interactable interactable) && interactable.CanInteract(gameObject))
                {
                    interactable.Interact();
                    break;
                }
                
                // if we hit a physical object, we are done.
                if (!hit.collider.isTrigger)
                    break;
            }
        }
    }
}
