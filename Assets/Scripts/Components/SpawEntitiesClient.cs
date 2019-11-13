using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ECS
{
    public enum EntityType
    {
        Player,
        Plate,
        Enemy2,
        RocketPlayer,
        RocketEnemy
    }

    [InternalBufferCapacity(128)]
    public struct SpawnEntityBuffer : IBufferElementData
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