using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ECS
{
    
    [Serializable]
    public unsafe struct SnapshotFromServer : IComponentData
    {
        public uint tick;
        public int length;
        public uint* data;
    }

}