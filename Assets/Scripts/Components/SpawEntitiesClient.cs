using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ECS
{
    public enum EntityType
    {
        Player,
        Enemy1,
        Enemy2,
        RocketPlayer,
        RocketEnemy
    }

    [InternalBufferCapacity(128)]
    public struct EntityBuffer : IBufferElementData
    {
        public int id;
        public EntityType type;
        public float3 pos;
        public quaternion rotation;
    }

    [Serializable]
    public struct SpawnEntitiesClient : IComponentData
    {
        
    }

}