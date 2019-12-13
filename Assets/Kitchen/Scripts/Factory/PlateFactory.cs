using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
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

            var position = new Translation() {Value = new float3() {x = 0.0f, y = -10f, z = 0.0f}};
            entityManager.SetComponentData(e, position);
            entityManager.AddComponentData(e, new ReplicatedEntityData
            {
                Id = -1,
                PredictingPlayerId = -1
            });

            entityManager.AddComponentData(e, new Plate());
            entityManager.AddComponentData(e, new ItemInterpolatedState
            {
                Position = position.Value,
                Rotation = Quaternion.identity,
                Owner = Entity.Null
            });

            entityManager.AddComponentData(e, new ItemPredictedState
            {
                Position = position.Value,
                Rotation = Quaternion.identity,
                Owner = Entity.Null
            });

            return e;
        }
    }
}