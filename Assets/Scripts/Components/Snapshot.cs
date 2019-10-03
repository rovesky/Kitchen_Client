using System;
using Unity.Entities;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public unsafe struct SnapshotTick : IComponentData
    {
        public uint tick;
        public int length;
        public uint* data;
    }

}