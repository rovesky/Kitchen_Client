using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;

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
            dstManager.AddComponentData(entity, new TriggerDestroy() { IsCollision = false });
        }
    }
}


[UpdateBefore(typeof(BuildPhysicsWorld))]
public class TriggerVolumeChangeMaterialSystem : ComponentSystem
{
    EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    EntityQuery m_OverlappingGroup;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_OverlappingGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(OverlappingTriggerVolume),
                typeof(DestroyOverlappingTriggerVolume),
            }
        });
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        var overlappingComponents = GetComponentDataFromEntity<OverlappingTriggerVolume>();
        var TriggerDestroyComponents = GetComponentDataFromEntity<TriggerDestroy>(true);

        using (var overlappingEntities = m_OverlappingGroup.ToEntityArray(Allocator.TempJob))
        {
            foreach (var entity in overlappingEntities)
            {
                var overlapComponent = overlappingComponents[entity];
                if (overlapComponent.HasJustEntered)
                {
                     PostUpdateCommands.DestroyEntity(entity);                 
                }
            }
        }
    }
}