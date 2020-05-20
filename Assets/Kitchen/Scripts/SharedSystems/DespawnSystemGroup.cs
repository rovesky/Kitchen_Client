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

                    if (EntityManager.HasComponent<UIObject>(entity))
                    { 
                        var uiObject = EntityManager.GetComponentData<UIObject>(entity);
                        
                        if (uiObject.Info != null)
                            uiObject.Info.SetActive(false);

                        if (uiObject.Icon != null)
                            uiObject.Icon.SetActive(false);

                        if (uiObject.Progress != null)
                            uiObject.Progress.SetActive(false);
                    }

                    if (EntityManager.HasComponent<ExtinguisherPresentation>(entity))
                    {
                        var extinguisherPresentation = EntityManager.GetComponentData<ExtinguisherPresentation>(entity);
                 
                        if(extinguisherPresentation.Smog != null)
                            extinguisherPresentation.Smog.SetActive(false);

                    }

                    if (EntityManager.HasComponent<PotPresentation>(entity))
                    {
                        var potPresentation = EntityManager.GetComponentData<PotPresentation>(entity);
                        if(potPresentation.Steam != null)
                            potPresentation.Steam.SetActive(false);
                    }
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
                        var obj = EntityManager.GetComponentData<CharacterPresentation>(entity).CharacterObject;
                        if (obj != null)
                            Object.Destroy(obj);
                    }

                    if (EntityManager.HasComponent<UIObject>(entity))
                    {
                        var uiObject = EntityManager.GetComponentData<UIObject>(entity);
                        
                        if (uiObject.Info != null)
                            Object.Destroy(uiObject.Info);

                        if (uiObject.Icon != null)
                            Object.Destroy(uiObject.Icon);

                        if (uiObject.Progress != null)
                            Object.Destroy(uiObject.Progress);
                    }


                    if (EntityManager.HasComponent<ExtinguisherPresentation>(entity))
                    {
                        var extinguisherPresentation = EntityManager.GetComponentData<ExtinguisherPresentation>(entity);
                        Object.Destroy(extinguisherPresentation.Smog);

                    }

                    if (EntityManager.HasComponent<PotPresentation>(entity))
                    {
                        var potPresentation = EntityManager.GetComponentData<PotPresentation>(entity);
                        Object.Destroy(potPresentation.Steam);

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