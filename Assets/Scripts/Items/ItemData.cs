using System;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public struct ItemData
    {
        public ItemType type;
        public Vector3 position;
        public Quaternion rotation;
    }
}
