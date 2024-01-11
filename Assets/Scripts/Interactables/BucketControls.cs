using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using pt_player_3d.Scripts.Interaction;
using UnityEngine;
using UnityEngine.Splines;

public enum BucketState
{
    AtBottom,
    AtTop,
}

public class BucketControls : Interactable
{
    [SerializeField] private SplineAnimate splineAnimator;
    [SerializeField] private float bottomValue;
    [SerializeField] private float topValue;

    private CancellationTokenSource _cts;
    
    private void Start()
    {
        splineAnimator.NormalizedTime = Game.Save.BucketState switch {
            BucketState.AtBottom => bottomValue,
            BucketState.AtTop => topValue,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public override void Interact()
    {
        base.Interact();
        BucketState targetState = Game.Save.BucketState == BucketState.AtBottom ? BucketState.AtTop : BucketState.AtBottom;
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        PlayBucketMovementAnimation(targetState, _cts.Token).Forget();
    }

    private async UniTask PlayBucketMovementAnimation(BucketState targetState, CancellationToken token = default)
    {
        Game.Save.BucketState = targetState;
        float start = splineAnimator.NormalizedTime;
        float end = targetState == BucketState.AtBottom ? bottomValue : topValue;
        float elapsed = 0;

        while (elapsed < splineAnimator.Duration)
        {
            if (token.IsCancellationRequested)
                return;
            
            elapsed += Time.deltaTime;
            float t = elapsed / splineAnimator.Duration;
            splineAnimator.NormalizedTime = Mathf.Lerp(start, end, t);
            await UniTask.Yield();
        }
    }
}