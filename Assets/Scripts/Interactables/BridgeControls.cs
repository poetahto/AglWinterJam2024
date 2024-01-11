using System;
using Cysharp.Threading.Tasks;
using pt_player_3d.Scripts.Interaction;
using UnityEngine;

public class BridgeControls : Interactable
{
    [SerializeField] private GameObject bridgeObject;
    [SerializeField] private float closedAngle;
    [SerializeField] private float openAngle;
    [SerializeField] private float duration;
    
    public event Action OnBridgeOpenStart;
    public event Action OnBridgeOpenEnd;
    public bool IsLocked { get; set; } = true;

    private void Start()
    {
        bridgeObject.transform.rotation = Quaternion.Euler(closedAngle, 0, 0);
    }

    public override bool CanInteract(GameObject source)
    {
        return base.CanInteract(source) && !IsLocked;
    }

    public override void Interact()
    {
        base.Interact();
        
        if (IsLocked)
            return;

        IsLocked = true;
        OnBridgeOpenStart?.Invoke();
        PlayBridgeAnimation(openAngle)
            .ContinueWith(() => OnBridgeOpenEnd?.Invoke())
            .Forget();
    }

    public void CloseBridge()
    {
        PlayBridgeAnimation(closedAngle)
            .Forget();
    }

    private async UniTask PlayBridgeAnimation(float targetAngle)
    {
        float elapsed = 0;
        Quaternion start = bridgeObject.transform.rotation;
        Quaternion end = Quaternion.Euler(targetAngle, 0, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            bridgeObject.transform.rotation = Quaternion.Lerp(start, end, t);
            await UniTask.Yield();
        }
    }
}
