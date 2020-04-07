using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class SpawnPlatesSystem : ComponentSystem
    {
        private bool isSpawned;
        private Entity platePrefab;

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

            var query = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(Table)
                },
                None = new ComponentType[]
                {
                    typeof(BoxSetting),
                    typeof(TableSlice)
                }
            });
            var entities = query.ToEntityArray(Allocator.TempJob);

            //生成Plate
            for (var i = 0; i < 3; ++i)
            {
                var entity = entities[i * 2];
                var slot = EntityManager.GetComponentData<SlotPredictedState>(entity);
                var slotData = EntityManager.GetComponentData<SlotSetting>(entity);

                var e = EntityManager.Instantiate(platePrefab);
                slot.FilledInEntity = e;
                EntityManager.SetComponentData(entity, slot);

                ItemCreateUtilities.CreateItemComponent(EntityManager, e,
                    slotData.Pos, Quaternion.identity);

                EntityManager.SetComponentData(e, new Plate());
            }

            entities.Dispose();
        }
    }
}