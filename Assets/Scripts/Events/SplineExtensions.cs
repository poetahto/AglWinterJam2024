using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Splines;
namespace Ltg8
{
    public static class SplineExtensions
    {
        public static async UniTask FollowPath(this GameObject gameObject, Spline spline, float speed = 5)
        {
            float elapsed = 0;
            float length = spline.GetLength();
            float duration = length / speed;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                gameObject.transform.position = spline.EvaluatePosition(t);
                await UniTask.Yield();
            }
        }
        
        public static async UniTask PlayAsTask(this SplineAnimate spline, CancellationToken token = default)
        {
            spline.Play();

            while (spline.IsPlaying && !token.IsCancellationRequested)
                await UniTask.Yield();
        }
    }
}
