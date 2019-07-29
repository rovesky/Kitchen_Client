using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public struct TriggerDestroy : IComponentData
    {
        public bool IsCollision;
    }

    public class TriggerDestroyBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        void OnEnable() { }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (enabled)
            {
               // dstManager.AddComponent(
                dstManager.AddComponentData(entity, new TriggerDestroy() { IsCollision = false });
            }
        }
    }


    //[UpdateBefore(typeof(BuildPhysicsWorld))]
    //public class TriggerVolumeChangeMaterialSystem : ComponentSystem
    //{
    //    EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    //    EntityQuery m_OverlappingGroup;

    //    protected override void OnCreate()
    //    {
    //        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    //        m_OverlappingGroup = GetEntityQuery(new EntityQueryDesc
    //        {
    //            All = new ComponentType[]
    //            {
    //                typeof(OverlappingTriggerVolume),
    //                typeof(DestroyOverlappingTriggerVolume),
    //            }
    //        });
    //    }

    //    [BurstCompile]
    //    protected override void OnUpdate()
    //    {
    //        var overlappingComponents = GetComponentDataFromEntity<OverlappingTriggerVolume>();
    //        var TriggerDestroyComponents = GetComponentDataFromEntity<TriggerDestroy>(true);
    //        //   var CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();
    //        using (var overlappingEntities = m_OverlappingGroup.ToEntityArray(Allocator.TempJob))
    //        {
    //            foreach (var entity in overlappingEntities)
    //            {
    //                var overlapComponent = overlappingComponents[entity];
    //                if (overlapComponent.HasJustEntered && TriggerDestroyComponents.Exists(entity))
    //                {
    //                    PostUpdateCommands.DestroyEntity(entity);
    //                    // CommandBuffer.DestroyEntity(entity);
    //                }
    //            }
    //        }
    //    }
}