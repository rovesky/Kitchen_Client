using FootStone.ECS;
using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ECS
{ 
    public struct LocalPlayer : IComponentData
    {
        public int playerId;
        public Entity playerEntity;
    
    }

}