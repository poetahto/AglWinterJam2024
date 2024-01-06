using System;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public struct ItemData
    {
        public ItemType Type;
        public Vector3 Position;
        public Quaternion Rotation;
    }
}
