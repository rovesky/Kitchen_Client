using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class SpawnPlatesSystem : ComponentSystem
    {
        private Entity platePrefab;
        private bool isSpawned = false;

        protected override void OnCreate()
        {
            base.OnCreate();

           platePrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
               Resources.Load("Plate") as GameObject,
               GameObjectConversionSettings.FromWorld(World,
                   World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>().BlobAssetStore));
        }

        protected override void OnUpdate()
        {
            if (isSpawned)
                return;

            isSpawned = true;

            var query = GetEntityQuery(typeof(TriggeredSetting));
            var entities = query.ToEntityArray(Allocator.TempJob);

            //生成Plate
            for (var i = 0; i < 3; ++i)
            {
                var entity = entities[i * 2];
                var slot = EntityManager.GetComponentData<SlotPredictedState>(entity);
                var triggerData = EntityManager.GetComponentData<TriggeredSetting>(entity);

                var e = EntityManager.Instantiate(platePrefab);
                slot.FilledInEntity = e;
                EntityManager.SetComponentData(entity, slot);
           
                CreateItemUtilities.CreateItemComponent(EntityManager, e, 
                    triggerData.SlotPos, Quaternion.identity);
            }

            entities.Dispose();

        }
    }
}