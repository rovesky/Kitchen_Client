using FootStone.ECS;
using Unity.Entities;


namespace FootStone.Kitchen
{
    public class MenuFactory : ReplicatedEntityFactory
    {

        public override Entity Create(EntityManager entityManager, BundledResourceManager resourceManager,
            GameWorld world,ushort type)
        {
            return  GameCreateUtilities.CreateMenuItem(entityManager);
        }
    }
}