using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public class ItemFactory : ReplicatedEntityFactory
    {

        public override Entity Create(EntityManager entityManager, BundledResourceManager resourceManager,
            GameWorld world, ushort type)
        {
            //已经有预测的item，直接返回
            var e = AttachPredictItem(entityManager, type);
            if (e != Entity.Null)
                return e;

            //没有预测的item，创建新的item
            e = ItemCreateUtilities.CreateItem(entityManager,
                (EntityType) type, new float3 {x = 0.0f, y = -30f, z = 0.0f}, Entity.Null);

            entityManager.AddComponentData(e, new NewServerEntity()
            {
                Tick = 1
            });
            entityManager.AddComponentData(e, new IconUI());
            return e;
        }

        private Entity AttachPredictItem(EntityManager entityManager, ushort type)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<GameEntity>(),
                ComponentType.ReadOnly<ReplicatedEntityData>(),
                ComponentType.ReadOnly<PredictedItem>());

            var entities = query.ToEntityArray(Allocator.TempJob);

            foreach (var entity in entities)
            {
                var replicatedEntityData = entityManager.GetComponentData<ReplicatedEntityData>(entity);
                var gameEntity = entityManager.GetComponentData<GameEntity>(entity);

                if ((ushort) gameEntity.Type != type || replicatedEntityData.Id != -1)
                    continue;

                entityManager.RemoveComponent<PredictedItem>(entity);
                entities.Dispose();

                FSLog.Info($"Attach Predict item:{entity}");
                return entity;
            }

            entities.Dispose();
            return Entity.Null;
        }
    }
}