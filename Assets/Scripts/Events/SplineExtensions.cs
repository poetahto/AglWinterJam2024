using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Splines;
namespace Ltg8
{
    public static class SplineExtensions
    {
        public static async UniTask PlayAsTask(this SplineAnimate spline, CancellationToken token = default)
        {
            spline.Play();

            while (spline.IsPlaying && !token.IsCancellationRequested)
                await UniTask.Yield();
        }
    }
}
