using System.Threading;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;
namespace Ltg8
{
    public class FlipBookSpeechAnimator : MonoBehaviour
    {
        [SerializeField] private FlipBookAnimation anim;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private EventInstance _currentInstance;
        private float _elapsed;
        private int _animIndex;
        private bool _wasPlaying;

        public async UniTask PlaySpeech(EventReference reference, CancellationToken token = default)
        {
            if (_currentInstance.isValid())
                _currentInstance.stop(STOP_MODE.IMMEDIATE);
            
            _currentInstance = RuntimeManager.CreateInstance(reference);
            _currentInstance.start();

            while (!token.IsCancellationRequested)
            {
                _currentInstance.getPlaybackState(out PLAYBACK_STATE state);
                
                if (state == PLAYBACK_STATE.STOPPED)
                    break;

                await UniTask.Yield();
            }

            _currentInstance.stop(STOP_MODE.IMMEDIATE);
        }

        private void Update()
        {
            if (_currentInstance.isValid())
            {
                _currentInstance.set3DAttributes(gameObject.To3DAttributes());
                _currentInstance.getPlaybackState(out PLAYBACK_STATE state);
                bool isPlaying = state == PLAYBACK_STATE.PLAYING;
                if (_wasPlaying && !isPlaying)
                {
                    // just stopped
                    spriteRenderer.sprite = anim.frames[0];
                }
                _wasPlaying = isPlaying;

                if (state == PLAYBACK_STATE.PLAYING)
                {
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
}
