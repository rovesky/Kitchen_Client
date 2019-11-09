using FootStone.ECS;
using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ECS
{
    
    [Serializable]
    public struct LocalPlayer : IComponentData
    {
        public int playerId;
        public Entity playerEntity;
    
    }

}