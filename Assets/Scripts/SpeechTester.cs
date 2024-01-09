using FMODUnity;
using UnityEngine;
namespace Ltg8
{
    public class SpeechTester : MonoBehaviour
    {
        public EventReference speech;
        
        private void Start()
        {
            GetComponent<FlipBookSpeechAnimator>().PlaySpeech(speech);
        }
    }
}
