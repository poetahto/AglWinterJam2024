using System;
using pt_player_3d.Scripts.Interaction;
using UnityEngine;
using UnityEngine.Splines;

public class BucketControls : Interactable
{
    [SerializeField] private SplineAnimate splineAnimator;
    [SerializeField] private float bottomValue;
    [SerializeField] private float topValue;
    [SerializeField] private float speed;

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
        Game.Save.BucketState = Game.Save.BucketState == BucketState.AtBottom ? BucketState.AtTop : BucketState.AtBottom;
    }

    private void Update()
    {
        float target = Game.Save.BucketState == BucketState.AtBottom ? bottomValue : topValue;
        splineAnimator.NormalizedTime = Mathf.MoveTowards(splineAnimator.NormalizedTime, target, speed * Time.deltaTime);
    }
}