using UnityEngine;
namespace Ltg8
{
    public class OverworldNpcSpawner : MonoBehaviour
    {
        public float minSpawnDelay = 10;
        public float maxSpawnDelay = 20;

        private float _remainingTime;

        public bool IsSpawning { get; set; } = false;

        private void Start()
        {
            CalculateNextSpawnTime();
        }

        private void Update()
        {
            if (!IsSpawning)
                return;

            _remainingTime -= Time.deltaTime;

            if (_remainingTime <= 0)
            {
                CalculateNextSpawnTime();
                
                // pick random npc to spawn
                Transform t = transform;
                NpcData npcData = new NpcData {
                    type = NpcType.Normal,
                    position = t.position,
                    rotation = t.rotation.eulerAngles,
                };
                Game.Overworld.NpcGenerator.SpawnNpc(npcData);
            }
        }

        private void CalculateNextSpawnTime()
        {
            _remainingTime = Random.Range(minSpawnDelay, maxSpawnDelay);
        }
    }
}
