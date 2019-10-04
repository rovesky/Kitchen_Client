using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ECS
{

    [InternalBufferCapacity(20)]
    public struct EnemyBuffer : IBufferElementData
    {
        public int id;
        public EnemyType type;
        public float3 pos;
    }

    [Serializable]
    public struct SpawnEnemyClient : IComponentData
    {
        
    }

}