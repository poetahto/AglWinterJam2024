using System.Collections.Generic;
using Core;
using DefaultNamespace;
using UnityEngine;

public class BucketItemHolder : MonoBehaviour
{
    private readonly HashSet<OverworldItemView> _heldItems = new HashSet<OverworldItemView>();
    private Vector3 _prevPos; 

    private void Update()
    {
        Vector3 curPos = transform.position;
        Vector3 velocity = curPos - _prevPos;
        _prevPos = curPos;
        
        foreach (OverworldItemView heldItem in _heldItems)
        {
            if (heldItem != null)
                heldItem.transform.position += velocity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponentWithRigidbody(out OverworldItemView item))
            _heldItems.Add(item);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponentWithRigidbody(out OverworldItemView item) && _heldItems.Contains(item))
            _heldItems.Remove(item);
    }
}
