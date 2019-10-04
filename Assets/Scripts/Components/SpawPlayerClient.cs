using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ECS
{

    [InternalBufferCapacity(20)]
    public struct PlayerClientBuffer : IBufferElementData
    {
        public int id;
        public float3 pos;
    }

    [Serializable]
    public struct SpawnPlayerClient : IComponentData
    {
        
    }

}