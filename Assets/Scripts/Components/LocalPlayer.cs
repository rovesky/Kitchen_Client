using FootStone.ECS;
using System;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{ 
    public struct LocalPlayer : IComponentData
    {
        public int playerId;
        public Entity playerEntity;
    
    }

}