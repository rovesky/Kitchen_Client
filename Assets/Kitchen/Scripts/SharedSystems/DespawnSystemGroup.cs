using FootStone.ECS;
using Unity.Entities;
using Unity.Physics;
using Unity.Rendering;
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
                .WithStructuralChanges().ForEach((Entity entity) =>
                {
                    EntityManager.RemoveComponent<RenderMesh>(entity);

                    if (EntityManager.HasComponent<PhysicsVelocity>(entity))
                        EntityManager.RemoveComponent<PhysicsVelocity>(entity);

                    if (EntityManager.HasComponent<PhysicsMass>(entity))
                        EntityManager.RemoveComponent<PhysicsMass>(entity);
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
                    if (EntityManager.HasComponent<CharacterPresentation>(entity))
                    {
                        var obj = EntityManager.GetComponentData<CharacterPresentation>(entity).Object;
                        if (obj != null)
                            Object.Destroy(obj);
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