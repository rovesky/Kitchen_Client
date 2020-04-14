using FootStone.ECS;
using Unity.Entities;


namespace FootStone.Kitchen
{
    public class GameFactory : ReplicatedEntityFactory
    {

        public GameFactory()
        {
           

        }

        public override Entity Create(EntityManager entityManager, BundledResourceManager resourceManager,
            GameWorld world,ushort type)
        {
            var e = entityManager.CreateEntity(typeof(ReplicatedEntityData),typeof(Countdown),typeof(Score));
             entityManager.SetComponentData(e,new ReplicatedEntityData()
            {
                Id = -1,
                PredictingPlayerId = -1
            });

            entityManager.SetComponentData(e,new Countdown()
            {
                Value = 0,
                EndTime = 0
            });

            entityManager.SetComponentData(e,new Score()
            {
                Value = 0
            });

            return e;
        }
    }
}