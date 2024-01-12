using System.Collections.Generic;
using poetools.Console;
using UnityEngine;
using Random = UnityEngine.Random;
namespace Ltg8
{
    public class OverworldEventFactory : MonoBehaviour
    {
        [ConsoleCommand("faster")]
        public static void Faster()
        {
            Time.timeScale *= 2;
        }
        
        [ConsoleCommand("slower")]
        public static void Slower()
        {
            Time.timeScale /= 2;
        }
        
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
            OverworldEvent randomEventPrefab = _eventPool[_index];
            OverworldEvent instance = Instantiate(randomEventPrefab);
            _index++;
            
            if (_index > _eventPool.Count - 1)
            {
                _index = 0;
                Shuffle();
            }
            
            return instance;
        }
    }
}
