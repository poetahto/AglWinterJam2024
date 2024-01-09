using UnityEngine;

namespace Ltg8
{
    public class FlipBookMovementAnimator : MonoBehaviour
    {
        [SerializeField] private FlipBookAnimation anim;
        [SerializeField] private float walkThreshold = 1;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private Vector3 _prevPos;
        private bool _wasWalking;
        private int _animIndex;
        private float _elapsed;

        private void Update()
        {
            Vector3 curPos = transform.position;
            Vector3 velocity = (curPos - _prevPos) / Time.deltaTime;
            float speed = velocity.magnitude;
            _prevPos = curPos;

            bool isWalking = speed >= walkThreshold;
            if (!_wasWalking && isWalking)
            {
                // just started
                _animIndex = 0;
                _elapsed = 0;
                spriteRenderer.sprite = anim.frames[0];
            }
            else if (_wasWalking && !isWalking)
            {
                // just stopped
                spriteRenderer.sprite = anim.frames[0];
            }
            _wasWalking = isWalking;

            if (isWalking)
            {
                // update animation
                if (_elapsed > anim.animationRate)
                {
                    _animIndex = (_animIndex + 1) % anim.frames.Length;
                    _elapsed = 0;
                    spriteRenderer.sprite = anim.frames[_animIndex];
                }
                _elapsed += Time.deltaTime;
            }
        }
    }
}
