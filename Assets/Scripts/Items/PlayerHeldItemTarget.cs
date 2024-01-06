using UnityEngine;
namespace DefaultNamespace
{
    public class PlayerHeldItemTarget : MonoBehaviour
    {
        private void OnEnable()
        {
            Game.SaveBlock.HasHeldItemTarget = true;
        }

        private void OnDisable()
        {
            Game.SaveBlock.HasHeldItemTarget = false;
        }

        private void Update()
        {
            Game.SaveBlock.HeldItemTarget = transform.position;
        }
    }
}
