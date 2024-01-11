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

    public event Action<BucketState> OnBucketStateChange;
    private CancellationTokenSource _cts;
    private BucketState _targetState;
    
    private void Start()
    {
        _targetState = Game.Save.BucketState;
        
        splineAnimator.NormalizedTime = Game.Save.BucketState switch {
            BucketState.AtBottom => bottomValue,
            BucketState.AtTop => topValue,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public override void Interact()
    {
        base.Interact();
        _targetState = _targetState == BucketState.AtBottom ? BucketState.AtTop : BucketState.AtBottom;
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        PlayBucketMovementAnimation(_cts.Token).Forget();
    }

    private async UniTask PlayBucketMovementAnimation(CancellationToken token = default)
    {
        float start = splineAnimator.NormalizedTime;
        float end = _targetState == BucketState.AtBottom ? bottomValue : topValue;
        float duration = splineAnimator.Duration;
        float elapsed = 0;

        while (elapsed < duration)
        {
            if (token.IsCancellationRequested)
                return;
            
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            splineAnimator.NormalizedTime = Mathf.Lerp(start, end, t);
            await UniTask.Yield();
        }
        
        Game.Save.BucketState = _targetState;
        OnBucketStateChange?.Invoke(_targetState);
    }
}