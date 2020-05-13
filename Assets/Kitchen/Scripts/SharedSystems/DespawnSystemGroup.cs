using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{

    /// <summary>
    /// 客户端despawn，设置为不可见
    /// </summary>
    [DisableAutoCreation]
    public class DespawnSystemClient : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<Despawn>()
                .WithStructuralChanges().
                ForEach((Entity entity) =>
                {
                    EntityManager.RemoveComponent<MeshRenderer>(entity);
                }).Run();
        }
    }


    /// <summary>
    /// 服务器确认过的需要despawn，删除该entity
    /// </summary>
    [DisableAutoCreation]
    public class DespawnSystemServer : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities
                .WithStructuralChanges()
                .WithAll<DespawnServer>()
                .ForEach((Entity entity) =>
            {
                if (EntityManager.HasComponent<Transform>(entity))
                {
                    Object.Destroy(EntityManager.GetComponentObject<Transform>(entity).gameObject);
                }

                EntityManager.DestroyEntity(entity);

            }).Run();
        }
    }

    [DisableAutoCreation]
    public class DespawnSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<DespawnSystemClient>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<DespawnSystemServer>());
        }
    }
}