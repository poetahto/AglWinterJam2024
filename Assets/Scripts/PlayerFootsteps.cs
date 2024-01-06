using FMODUnity;
using pt_player_3d.Scripts;
using UnityEngine;

namespace Ltg8
{
    public class PlayerFootsteps : MonoBehaviour
    {
        public float stepDistance = 1;
        public EventReference footstepSfx;

        private GroundCheck3d _groundCheck3d;
        private float _elapsed;
        private Vector3 _prevPos;

        private void Awake()
        {
            _groundCheck3d = GetComponent<GroundCheck3d>();
            _prevPos = transform.position;
        }

        private void Update()
        {
            if (_elapsed > stepDistance)
            {
                _elapsed = 0;
                RuntimeManager.PlayOneShot(footstepSfx);
            }
            
            Vector3 curPos = transform.position;
            
            if (_groundCheck3d.IsGrounded)
                _elapsed += Vector3.Distance(curPos, _prevPos);

            _prevPos = curPos;
        }
    }
}
