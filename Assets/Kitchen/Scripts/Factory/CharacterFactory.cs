using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;


namespace FootStone.Kitchen
{
    public class CharacterFactory : ReplicatedEntityFactory
    {

        public CharacterFactory()
        {

        }

        public override Entity Create(EntityManager entityManager, BundledResourceManager resourceManager,
            GameWorld world, ushort type)
        {
            var e = ClientCharacterUtilities.CreateCharacter(entityManager, new float3 {x = 0, y = -20, z = 9});
            entityManager.AddComponentData(e, new UpdateUI());
            return e;
        }
    }
}