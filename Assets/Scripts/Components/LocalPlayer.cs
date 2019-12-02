using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct LocalPlayer : IComponentData
    {
        public int PlayerId;
        public Entity PlayerEntity;
    }
}