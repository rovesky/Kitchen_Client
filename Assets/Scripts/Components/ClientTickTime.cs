using FootStone.ECS;
using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ECS
{

    public struct ClientTickTime : IComponentData
    {
        public GameTick predict;
        public GameTick render;
    }

}