using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
namespace Ltg8
{
    public class OverworldEventFactory : MonoBehaviour
    {
        public OverworldEventGroup eventGroup;

        private List<OverworldEvent> _eventPool = new List<OverworldEvent>();
        private int _index;

        private void Start()
        {
            foreach (OverworldEvent groupPrefab in eventGroup.prefabs)
                _eventPool.Add(groupPrefab);
        }

        private void Shuffle()
        {
            for (int i = 0; i < _eventPool.Count; i++)
            {
                int randomIndex = Random.Range(0, _eventPool.Count);
                (_eventPool[i], _eventPool[randomIndex]) = (_eventPool[randomIndex], _eventPool[i]);
            }
        }

        public OverworldEvent SpawnEvent()
        {
            OverworldEvent randomEventPrefab = eventGroup.prefabs[_index];
            OverworldEvent instance = Instantiate(randomEventPrefab);
            _index++;
            
            if (_index > _eventPool.Count)
            {
                _index = 0;
                Shuffle();
            }
            
            return instance;
        }
    }
}
