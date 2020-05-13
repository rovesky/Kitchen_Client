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
            var e = ClientCharacterUtilities.CreateCharacter(entityManager, float3.zero);
            entityManager.AddComponentData(e, new NewClientEntity());
        //    entityManager.AddComponentData(e, new UpdateUI());
            return e;
        }
    }
}