using Ltg8;
using UnityEngine;
namespace DefaultNamespace
{
    public class OverworldBehavior : MonoBehaviour
    {
        public ItemSystem Items => Game.Overworld.Items;
        public DecisionSystem Decisions => Game.Overworld.Decisions;
        public DayNightSystem DayNight => Game.Overworld.DayNight;
        public OverworldEventFactory EventFactory => Game.Overworld.EventFactory;
        
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
