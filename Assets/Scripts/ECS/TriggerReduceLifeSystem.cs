//using System.Threading;
//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Physics.Systems;
//using UnityEngine;


//[UpdateBefore(typeof(BuildPhysicsWorld))]
//public class TriggerReduceLifeSystem : ComponentSystem
//{
//   // EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
//    EntityQuery m_OverlappingGroup;

//    protected override void OnCreate()
//    {
//       // m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
//        m_OverlappingGroup = GetEntityQuery(new EntityQueryDesc
//        {
//            All = new ComponentType[]
//            {
//                typeof(OverlappingTriggerVolume),
//                typeof(ReduceLifeOverlappingTriggerVolume),
//            }
//        });
//    }

//    [BurstCompile]
//    protected override void OnUpdate()
//    {
//        var overlappingComponents = GetComponentDataFromEntity<OverlappingTriggerVolume>();
//        var LifeComponents = GetComponentDataFromEntity<Life>();
//        var AttackComponents = GetComponentDataFromEntity<Attack>(true);
//        //   var CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();
//        using (var overlappingEntities = m_OverlappingGroup.ToEntityArray(Allocator.TempJob))
//        {
//          //  Debug.Log("TriggerReduceLifeSystem.update");
//            foreach (var entity in overlappingEntities)
//            {
//                var overlapComponent = overlappingComponents[entity];
//              //  Debug.Log($"AttackComponents.Exists(entity):{AttackComponents.Exists(entity)}");
//              //  Debug.Log($"LifeComponents.Exists(entity):{LifeComponents.Exists(overlapComponent.VolumeEntity)}");
//                if (overlapComponent.HasJustEntered 
//                    && AttackComponents.Exists(entity)
//                    && LifeComponents.Exists(overlapComponent.VolumeEntity))
//                {
//                    var lifeComponent = LifeComponents[overlapComponent.VolumeEntity];
//                    lifeComponent.lifeValue -= AttackComponents[entity].Power;
                  
//                    LifeComponents[overlapComponent.VolumeEntity] = lifeComponent;                  
//                }

//                if(overlapComponent.HasJustEntered && LifeComponents.Exists(entity))
//                {
//                    Debug.Log($"lifeComponent.lifeValue--,{Thread.CurrentThread.ManagedThreadId}");
//                    var lifeComponent = LifeComponents[entity];
//                    lifeComponent.lifeValue -= 1;
//                    LifeComponents[entity] = lifeComponent;
//                }
//            }
//        }
//    }
//}