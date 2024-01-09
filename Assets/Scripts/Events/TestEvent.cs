using UnityEngine;
using UnityEngine.AI;
namespace Ltg8
{
    public class TestEvent : OverworldEvent
    {
        [SerializeField] private NavMeshAgent npc;
        [SerializeField] private Transform bucketPosition;
        [SerializeField] private Transform preGatePosition;
        
        private bool _isDone;
        public override bool IsDone => _isDone;

        private void Start()
        {
            // walk up to the bridge.
            // declare name, and ask to be let through.
            // [ask for id], [ask for money], [ask why].
        }
    }
}
