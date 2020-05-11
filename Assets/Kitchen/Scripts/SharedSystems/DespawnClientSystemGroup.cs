using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class DespawnSystemClient : ComponentSystem
    {
        private ReplicateEntitySystemGroup replicateEntitySystemGroup;

        protected override void OnCreate()
        {
            replicateEntitySystemGroup = World.GetExistingSystem<ReplicateEntitySystemGroup>();
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref Despawn despawn) =>
            {
                if (despawn.Frame <= 0)
                {
                    if(!EntityManager.HasComponent<PredictedItem>(entity) && replicateEntitySystemGroup != null)
                        replicateEntitySystemGroup.Unregister(entity);

                    if (EntityManager.HasComponent<Transform>(entity))
                    {
                        Object.Destroy(EntityManager.GetComponentObject<Transform>(entity).gameObject);
                    }

                    EntityManager.DestroyEntity(entity);
                  
                }

                despawn.Frame--;
            });
        }
    }
    [DisableAutoCreation]
    public class DespawnClientSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<DespawnSystemClient>());
        }
    }
}