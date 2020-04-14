using System.Collections.Generic;
using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class FoodFactory : ReplicatedEntityFactory
    {
        //private readonly Entity applePrefab;

        private Dictionary<EntityType,Entity> prefabs = new Dictionary<EntityType, Entity>();

        private void RegisterPrefabs(EntityType type, string res)
        {
            prefabs[type] = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load(res) as GameObject,
                GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld,
                    World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>().BlobAssetStore));

        }

        public FoodFactory()
        {
            RegisterPrefabs(EntityType.Shrimp, "Shrimp");
            RegisterPrefabs(EntityType.ShrimpSlice, "ShrimpSlice");
            RegisterPrefabs(EntityType.KelpSlice, "KelpSlice");
            RegisterPrefabs(EntityType.Rice, "Rice");
            RegisterPrefabs(EntityType.Cucumber, "Cucumber");
            RegisterPrefabs(EntityType.CucumberSlice, "CucumberSlice");
        }

        public override Entity Create(EntityManager entityManager, BundledResourceManager resourceManager,
            GameWorld world,ushort type)
        {
            EntityType entityType = (EntityType) type;
            if (!prefabs.ContainsKey(entityType))
                return Entity.Null;

            var e = entityManager.Instantiate(prefabs[entityType]);

            ItemCreateUtilities.CreateItemComponent(entityManager, e,
                new float3 {x = 0.0f, y = -10f, z = 0.0f}, quaternion.identity);

            entityManager.AddComponentData(e, new Food());

            entityManager.SetComponentData(e, new ReplicatedEntityData
            {
                Id = -1,
                PredictingPlayerId = -1
            });

            return e;
        }
    }
}