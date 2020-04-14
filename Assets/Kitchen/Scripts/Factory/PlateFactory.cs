using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
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
            GameWorld world,ushort type)
        {
            var e = entityManager.Instantiate(platePrefab);

            ItemCreateUtilities.CreateItemComponent(entityManager, e,
                new float3 {x = 0.0f, y = -10f, z = 0.0f}, quaternion.identity);
            

            entityManager.AddComponentData(e, new Plate());
            entityManager.AddComponentData(e, new PlatePredictedState()
            {
                Material1 = Entity.Null,
                Material2 = Entity.Null,
                Material3 = Entity.Null,
                Material4 = Entity.Null
            });

            entityManager.SetComponentData(e, new ReplicatedEntityData
            {
                Id = -1,
                PredictingPlayerId = -1
            });

            return e;
        }
    }
}