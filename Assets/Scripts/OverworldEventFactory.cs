using UnityEngine;
namespace Ltg8
{
    public class OverworldEventFactory : MonoBehaviour
    {
        public OverworldEventGroup eventGroup;

        public OverworldEvent SpawnEvent()
        {
            OverworldEvent randomEventPrefab = eventGroup.prefabs[Random.Range(0, eventGroup.prefabs.Count)];
            return Instantiate(randomEventPrefab);
        }
    }
}
