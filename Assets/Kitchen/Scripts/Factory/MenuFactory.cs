using FootStone.ECS;
using Unity.Entities;


namespace FootStone.Kitchen
{
    public class MenuFactory : ReplicatedEntityFactory
    {

        public MenuFactory()
        {
           

        }

        public override Entity Create(EntityManager entityManager, BundledResourceManager resourceManager,
            GameWorld world,ushort type)
        {
            var e = entityManager.CreateEntity(typeof(ReplicatedEntityData),typeof(MenuItem));
             entityManager.SetComponentData(e,new ReplicatedEntityData()
            {
                Id = -1,
                PredictingPlayerId = -1
            });

            entityManager.SetComponentData(e,new MenuItem());
         //   entityManager.AddComponentData(e,new NewEntity());

            return e;
        }
    }
}