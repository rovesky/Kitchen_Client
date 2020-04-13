﻿using System.Collections.Generic;
using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class SpawnFoodsSystem : ComponentSystem
    {
        private Dictionary<EntityType,Entity> prefabs = new Dictionary<EntityType, Entity>();

        private void RegisterPrefabs(EntityType type, string res)
        {
            prefabs[type] = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load(res) as GameObject,
                GameObjectConversionSettings.FromWorld(World,
                    World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>().BlobAssetStore));

        }

        protected override void OnCreate()
        {
            base.OnCreate();

            var entity = EntityManager.CreateEntity(typeof(SpawnFoodArray));
            SetSingleton(new SpawnFoodArray());
            EntityManager.AddBuffer<SpawnFoodRequest>(entity);

            RegisterPrefabs(EntityType.Shrimp, "Shrimp");
            RegisterPrefabs(EntityType.ShrimpSlice, "ShrimpSlice");
            RegisterPrefabs(EntityType.KelpSlice, "KelpSlice");
            RegisterPrefabs(EntityType.Rice, "Rice");
            RegisterPrefabs(EntityType.Cucumber, "Cucumber");
            RegisterPrefabs(EntityType.CucumberSlice, "CucumberSlice");


        }

        protected override void OnUpdate()
        {
          
            var entity = GetSingletonEntity<SpawnFoodArray>();
            var buffer = EntityManager.GetBuffer<SpawnFoodRequest>(entity);
            if (buffer.Length == 0)
                return;

            var array = buffer.ToNativeArray(Allocator.Temp);
            buffer.Clear();

            foreach (var spawnFood in array)
            {
                if(!prefabs.ContainsKey(spawnFood.Type))
                    continue;

                FSLog.Info($"spawnFood:{spawnFood.Type}");

                var prefab = prefabs[spawnFood.Type];
                var e = EntityManager.Instantiate(prefab);

                ItemCreateUtilities.CreateItemComponent(EntityManager, e,
                    spawnFood.Pos, quaternion.identity);

                if (spawnFood.Owner != Entity.Null)
                {
                    EntityManager.SetComponentData(e, new ItemPredictedState()
                    {
                        Owner = spawnFood.Owner,
                        PreOwner = Entity.Null
                    });

                    EntityManager.SetComponentData(spawnFood.Owner,new SlotPredictedState()
                    {
                        FilledInEntity = e
                    });
                }

                EntityManager.AddComponentData(e, new Food()
                {
                    Type = spawnFood.Type
                });

                if(spawnFood.IsSlice)
                    EntityManager.AddComponentData(e, new Slice());

                EntityManager.SetComponentData(e, new ReplicatedEntityData
                {
                    Id = -1,
                    PredictingPlayerId = -1
                });
            }

            array.Dispose();
        }
    }
}