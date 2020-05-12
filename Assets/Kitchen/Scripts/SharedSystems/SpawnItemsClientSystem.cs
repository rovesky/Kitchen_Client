using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;


namespace FootStone.Kitchen
{
    public struct PredictedItem : IComponentData
    {
        public uint StartTick;
    }
    

    [DisableAutoCreation]
    public class RemoveMispredictedItemsSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var serverTick = GetSingleton<ServerSnapshot>().ServerTick;
            Entities
                .WithName("RemoveMispredictedItems")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                        in PredictedItem predictedItem) =>
                {
                    if (predictedItem.StartTick < serverTick)
                        EntityManager.AddComponentData(entity, new Despawn());
                }).Run();
        }
    }


    [DisableAutoCreation]
    public class SpawnItemsClientSystem : SystemBase
    {

        protected override void OnCreate()
        {
            base.OnCreate();

            var entity = EntityManager.CreateEntity(typeof(SpawnItemArray));
            SetSingleton(new SpawnItemArray());
            EntityManager.AddBuffer<SpawnItemRequest>(entity);
        }

        protected override void OnUpdate()
        {
            var entity = GetSingletonEntity<SpawnItemArray>();
            var requests = EntityManager.GetBuffer<SpawnItemRequest>(entity);

          //  FSLog.Info($"Spwan item :{requests.Length}");
            if (requests.Length == 0)
                return;

            var array = requests.ToNativeArray(Allocator.Temp);
            requests.Clear();

            var query =  EntityManager.CreateEntityQuery(ComponentType.ReadOnly<PredictedItem>());
            var predictedItemEntities = query.ToEntityArray(Allocator.TempJob);


            foreach (var t in array)
            {
                var spawnItem = t;
                if (spawnItem.DeferFrame > 0)
                {
                    spawnItem.DeferFrame = spawnItem.DeferFrame - 1;
                    requests.Add(spawnItem);
                    continue;
                }

              
                //排除掉重返的request
                var isRepeatRequest = false;
                foreach (var predictedItemEntity in predictedItemEntities)
                {
                    var predictedItem = EntityManager.GetComponentData<PredictedItem>(predictedItemEntity);
                    var ownerState = EntityManager.GetComponentData<OwnerPredictedState>(predictedItemEntity);

                    if(predictedItem.StartTick == spawnItem.StartTick &&
                       ownerState.Owner == spawnItem.Owner)
                        isRepeatRequest = true;
                }
                if(isRepeatRequest)
                    continue;
      
                FSLog.Info($"Spawn item:{spawnItem.Type}");

                var e = ItemCreateUtilities.CreateItem(EntityManager, spawnItem.Type,
                    spawnItem.OffPos, spawnItem.Owner);

                if (e == Entity.Null)
                    continue;

                EntityManager.AddComponentData(e, new PredictedItem()
                {
                    StartTick = spawnItem.StartTick
                });

                if (spawnItem.Owner == Entity.Null)
                    continue;

                ItemAttachUtilities.ItemAttachToOwner(EntityManager,
                    e, spawnItem.Owner, Entity.Null);
            }

            predictedItemEntities.Dispose();
            array.Dispose();
        }
    }
  

    [DisableAutoCreation]
    public class ItemClientSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<RemoveMispredictedItemsSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<SpawnItemsClientSystem>());

        }
    }
}