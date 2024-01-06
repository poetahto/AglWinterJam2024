using UnityEngine;
namespace DefaultNamespace
{
    public class PlayerHeldItemTarget : MonoBehaviour
    {
        private void OnEnable()
        {
            Game.Save.HasHeldItemTarget = true;
        }

        private void OnDisable()
        {
            Game.Save.HasHeldItemTarget = false;
        }

        private void Update()
        {
            Game.Save.HeldItemTarget = transform.position;
        }
    }
}
