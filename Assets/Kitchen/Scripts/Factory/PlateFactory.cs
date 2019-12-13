using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class PlateFactory : ReplicatedEntityFactory
    {
        private readonly Entity platePrefab;

        public PlateFactory()
        {
             platePrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Plate") as GameObject,
                GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld,
                    World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>().BlobAssetStore));

        }

        public override Entity Create(EntityManager entityManager, BundledResourceManager resourceManager,
            GameWorld world)
        {
        

            var e = entityManager.Instantiate(platePrefab);

            entityManager.AddComponentData(e, new ReplicatedEntityData
            {
                Id = -1,
                PredictingPlayerId = -1
            });

            entityManager.AddComponentData(e, new Plate());
            entityManager.AddComponentData(e, new ItemInterpolatedState
            {
                Position = Vector3.zero,
                Rotation = Quaternion.identity,
                Owner = Entity.Null
            });

            entityManager.AddComponentData(e, new ItemPredictedState
            {
                Position = Vector3.zero,
                Rotation = Quaternion.identity,
                Owner = Entity.Null
            });

            return e;
        }
    }
}