using pt_player_3d.Scripts.Rotation;
using UnityEngine;

namespace Ltg8
{
    public class PlayerSaveTracker : MonoBehaviour
    {
        private RotationSystem _rotationSystem;
        
        private void Start()
        {
            Game.Save.PlayerScene = gameObject.scene.name;
            _rotationSystem = GetComponent<RotationSystem>();
        }
        
        private void Update()
        {
            Transform t = transform;
            Game.Save.PlayerPosition = t.position;
            Game.Save.PlayerRotation = _rotationSystem.Rotation;
        }
    }
}
