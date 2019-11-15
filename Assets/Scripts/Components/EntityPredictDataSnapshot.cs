using FootStone.ECS;
using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct EntityPredictDataSnapshot : IComponentData
    {
        public float3 position;
        public quaternion rotation;
        public Entity pickupEntity;
     
    }
}