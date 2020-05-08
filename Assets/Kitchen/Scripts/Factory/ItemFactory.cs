using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace FootStone.Kitchen
{
    public class ItemFactory : ReplicatedEntityFactory
    {

        public override Entity Create(EntityManager entityManager, BundledResourceManager resourceManager,
            GameWorld world, ushort type)
        {
            var e =  ItemCreateUtilities.CreateItem(entityManager,
                (EntityType) type, new float3 {x = 0.0f, y = -10f, z = 0.0f},Entity.Null);
        
       
            return e;
        }
    }
}