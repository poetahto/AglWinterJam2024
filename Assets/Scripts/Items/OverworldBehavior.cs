using UnityEngine;
namespace DefaultNamespace
{
    public class OverworldBehavior : MonoBehaviour
    {
        protected virtual void Start()
        {
            if (Game.Overworld.IsLoaded)
                OnStartOverworld();
        }

        public virtual void OnStartOverworld()
        {
        }
    }
}
