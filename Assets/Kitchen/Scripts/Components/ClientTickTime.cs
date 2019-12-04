using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct ClientTickTime : IComponentData
    {
        public GameTick Predict;
        public GameTick Render;
    }
}