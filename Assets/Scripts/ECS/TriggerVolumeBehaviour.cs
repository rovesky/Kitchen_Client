//using Unity.Physics;
//using Unity.Physics.Systems;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using UnityEngine;
//using Unity.Burst;
//using System;
//using UnityEditor;
//using System.Collections.Generic;
//using System.Threading;

//[Flags]
//public enum TriggerVolumeType 
//{
//    None = 0,
//    Destroy = 1<<0,
//    ReduceLife = 1<<1,
//    DestoryReduceLife = ReduceLife | Destroy,  
//}

//public struct OverlappingTriggerVolume : IComponentData
//{
//    public Entity VolumeEntity;
//    public int VolumeType;
//    public int CreatedFrame;
//    public int CurrentFrame;

//    public bool HasJustEntered { get { return (CurrentFrame - CreatedFrame) == 0; } }
//}

//public struct DestroyOverlappingTriggerVolume : IComponentData { }
//public struct ReduceLifeOverlappingTriggerVolume : IComponentData { }
////public struct ForceFieldOverlappingTriggerVolume : IComponentData { }


//public struct TriggerVolume : IComponentData
//{
//    public int Type;
//    public int CurrentFrame;
//}

//public class TriggerVolumeBehaviour : MonoBehaviour, IConvertGameObjectToEntity
//{
//    public TriggerVolumeType Type = TriggerVolumeType.None;

//    void OnEnable() { }

//    void OnGui()
//    {
//#if UNITY_EDITOR
//        Type = (TriggerVolumeType)EditorGUILayout.EnumFlagsField(Type);
//#endif
//    }

//    void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//        if (enabled)
//        {
//            dstManager.AddComponentData(entity, new TriggerVolume()
//            {
//                Type = (int)Type,
//                CurrentFrame = 0,
//            });
//        }
//    }
//}


//// A system which adds caching information to dynamic bodies that enter, persist and leave
//// a Trigger Volume. 
//// A Trigger Volume is defined by a PhysicsShape with the `Is Trigger` flag ticked and a
//// TriggerVolume behaviour added. Each trigger volume can be a different type. 
//// When a dynamic body enters the Trigger Volume an OverlappingTriggerVolume component is added
//// to it associated Entity, along with a tag component to say which type of volume it has entered.
//// The Volume entity and the Overlapping entity have internal frame counters. 
//// If the Overlapping entity's created frame matches the Volumes frame counter, it has just entered the Volume.
//// If the Overlapping entity's current frame no longer matches the Volumes frame counter, it has just left the Volume.

//// The current logic assumes that an entity will only be within a single Trigger Volume at any one time.
//// It also assumes that within one frame the Overlapping entity doesn't move from one Trigger Volume 
//// to another in a single frame. 
//// More data & logic would be needed to handle multiple overlaps, single frame Volume jumping and single entering & leaving.
//[UpdateAfter(typeof(EndFramePhysicsSystem))]
// public class TriggerVolumeSystem : JobComponentSystem
//{
//    EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

//    BuildPhysicsWorld m_BuildPhysicsWorldSystem;
//    StepPhysicsWorld m_StepPhysicsWorldSystem;

//    NativeArray<int> m_TriggerEntitiesIndex;

//    EntityQuery TriggerGroup;
//    EntityQuery OverlappingGroup;

//    protected override void OnCreate()
//    {
//        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

//        m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
//        m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
//        TriggerGroup = GetEntityQuery(new EntityQueryDesc
//        {
//            All = new ComponentType[] { typeof(TriggerVolume), }
//        });
//        OverlappingGroup = GetEntityQuery(new EntityQueryDesc
//        {
//            Any = new ComponentType[] {
//                typeof(OverlappingTriggerVolume),
//            }
//        });

//        m_TriggerEntitiesIndex = new NativeArray<int>(1, Allocator.Persistent);
//        m_TriggerEntitiesIndex[0] = 0;
//    }

//    protected override void OnDestroy()
//    {
//        m_TriggerEntitiesIndex.Dispose();
//    }

//    struct TriggerEntities
//    {
//        public Entity VolumeEntity;
//        public Entity OverlappingEntity;
//    }

//    [BurstCompile]
//    struct UpdateTriggerVolumesJob : IJobForEach<TriggerVolume>
//    {
//        public void Execute(ref TriggerVolume volumeComponent)
//        {
//            // Increment the frame count for all TriggerVolumes
//            // This frame count is compared with the overlapping entity
//            // frame count so that we can tell if an entity has left the volume.
//            volumeComponent.CurrentFrame++;

//         //   Debug.Log($"UpdateTriggerVolumesJob:{volumeComponent.CurrentFrame},{Thread.CurrentThread.ManagedThreadId}");
//        }
//    }


//    [BurstCompile]
//    struct GetTriggerEventCount : ITriggerEventsJob
//    {
//        [NativeFixedLength(1)] public NativeArray<int> pCounter;
        
//        // unsafe writing to this?
//        public void Execute(TriggerEvent triggerEvent)
//        {
           
//            pCounter[0]++;

//            Debug.Log($"GetTriggerEventCount,count:{ pCounter[0]}," +
//                $"EntityA:{triggerEvent.Entities.EntityA.Index},"+
//                $"EntityB:{triggerEvent.Entities.EntityB.Index}," +
//                $"thread:{Thread.CurrentThread.ManagedThreadId}");
//        }
//    }


//    [BurstCompile]
//    struct ListTriggerEventEntitiesJob : ITriggerEventsJob
//    {
//        [ReadOnly] public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityGroup;
//        [ReadOnly] public ComponentDataFromEntity<TriggerVolume> VolumeGroup;

//        [NativeFixedLength(1)] public NativeArray<int> pCounter;
//        public NativeArray<TriggerEntities> TriggerEntities;

//        public  void Execute(TriggerEvent triggerEvent)
//        {
//          //  Debug.Log($"TriggerEvent:{triggerEvent.Entities.EntityA.Index},{triggerEvent.Entities.EntityB.Index}");

//            Entity entityA = triggerEvent.Entities.EntityA;
//            Entity entityB = triggerEvent.Entities.EntityB;

//            bool isBodyATrigger = VolumeGroup.Exists(entityA);
//            bool isBodyBTrigger = VolumeGroup.Exists(entityB);

//            // Ignoring Triggers overlapping other Triggers
//            if (isBodyATrigger && isBodyBTrigger)
//                return;

//            bool isBodyADynamic = PhysicsVelocityGroup.Exists(entityA);
//            bool isBodyBDynamic = PhysicsVelocityGroup.Exists(entityB);

//            // Ignoring overlapping static bodies
//            if ((isBodyATrigger && !isBodyBDynamic) ||
//                (isBodyBTrigger && !isBodyADynamic))
//                return;

//            // Increment the output counter in a thread safe way.
//            var count = ++pCounter[0] - 1;

//            TriggerEntities[count] = new TriggerEntities()
//            {
//                VolumeEntity = isBodyATrigger ? entityA : entityB,
//                OverlappingEntity = isBodyATrigger ? entityB : entityA,
//            };
//            Debug.Log($"ListTriggerEventEntitiesJob,count:{TriggerEntities.Length}"+
//                $"EntityA:{triggerEvent.Entities.EntityA.Index}," +
//                $"EntityB:{triggerEvent.Entities.EntityB.Index}," +
//                $"thread:{Thread.CurrentThread.ManagedThreadId}");
//        }
//    }

//    [BurstCompile]
//    struct UpdateOverlappingJob : IJob
//    {
//        [ReadOnly] public NativeArray<TriggerEntities> TriggerEntites;
//        [NativeFixedLength(1)] [ReadOnly] public NativeArray<int> TriggerEntitiesCount;
//        public ComponentDataFromEntity<OverlappingTriggerVolume> OverlappingGroup;

//        public void Execute()
//        {
//            for (int index = 0; index < TriggerEntitiesCount[0]; index++)
//            {
//                var entities = TriggerEntites[index];
//                // Increment the frame count only for those entities that are 
//                // still in the TriggerEvent list. 
//                // TODO: this assumes overlapping of only one TriggerVolume at a time.
//                // TODO: we should really compare the entities.VolumeEntity is the
//                //       same as that saved in the component. The OverlappingEntity
//                //       may have jumped from inside one TriggerVolume to another in 
//                //       a single frame
//                if (OverlappingGroup.Exists(entities.OverlappingEntity))
//                {
//                     var component = OverlappingGroup[entities.OverlappingEntity];
//                    component.CurrentFrame++;
//                    OverlappingGroup[entities.OverlappingEntity] = component;
//                    Debug.Log($"UpdateOverlappingJob,CurrentFrame: {component.CurrentFrame},"+
//                    $"OverlappingEntity:{entities.OverlappingEntity.Index}," +                  
//                    $"thread:{Thread.CurrentThread.ManagedThreadId}");
//                }
//            }
//        }
//    }

//    //[BurstCompile]
//    struct AddNewOverlappingJob : IJob
//    {
//        public EntityCommandBuffer CommandBuffer;

//        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<TriggerEntities> TriggerEntities;
//        [NativeFixedLength(1)] [ReadOnly] public NativeArray<int> TriggerEntitiesCount;
//        [ReadOnly] public ComponentDataFromEntity<OverlappingTriggerVolume> OverlappingGroup;
//        [ReadOnly] public ComponentDataFromEntity<TriggerVolume> TriggerGroup;

//        public void Execute()
//        {
//            if (TriggerEntitiesCount.Length <= 0)
//                return;

//            Dictionary<Entity, bool> IsAdded = new Dictionary<Entity, bool>();
//            for (int i = 0; i < TriggerEntitiesCount[0]; i++)
//            {
//                var entities = TriggerEntities[i];
//                var overlappingEntity = entities.OverlappingEntity;

//              //  Debug.Log($"entities:{entities.OverlappingEntity.Index},{entities.VolumeEntity.Index}," +
//                 //    $",{OverlappingGroup.Exists(overlappingEntity)}");

//                // If the entity is in the collated list but does not have the associate
//                // OverlappingEntity component, then it has entered the TriggerVolume in this
//                // frame.

//                if (!OverlappingGroup.Exists(overlappingEntity) && !IsAdded.ContainsKey(overlappingEntity))
//                {
//                    var triggerComponent = TriggerGroup[entities.VolumeEntity];

//                    Debug.Log($"AddNewOverlappingJob,overlappingEntity:{overlappingEntity.Index},CurrentFrame:{triggerComponent.CurrentFrame}" +
//                        $",thread:{Thread.CurrentThread.ManagedThreadId}");

//                    var overlapComponent = new OverlappingTriggerVolume()
//                    {
//                        VolumeEntity = entities.VolumeEntity,
//                        VolumeType = triggerComponent.Type,
//                        CreatedFrame = triggerComponent.CurrentFrame,
//                        CurrentFrame = triggerComponent.CurrentFrame,
//                    };
//                    CommandBuffer.AddComponent<OverlappingTriggerVolume>(overlappingEntity, overlapComponent);
//                    IsAdded[overlappingEntity] = true;
//                    switch ((TriggerVolumeType)triggerComponent.Type)
//                    {
//                        case TriggerVolumeType.Destroy:
//                          //  CommandBuffer.DestroyEntity(overlappingEntity);
//                            CommandBuffer.AddComponent<DestroyOverlappingTriggerVolume>(overlappingEntity, new DestroyOverlappingTriggerVolume());
//                            break;
//                        case TriggerVolumeType.ReduceLife:
//                            CommandBuffer.AddComponent<ReduceLifeOverlappingTriggerVolume>(overlappingEntity, new ReduceLifeOverlappingTriggerVolume());
//                            break;
//                        case TriggerVolumeType.DestoryReduceLife:
//                            CommandBuffer.AddComponent<ReduceLifeOverlappingTriggerVolume>(overlappingEntity, new ReduceLifeOverlappingTriggerVolume());
//                            CommandBuffer.AddComponent<DestroyOverlappingTriggerVolume>(overlappingEntity, new DestroyOverlappingTriggerVolume());
//                            break;
//                        //case TriggerVolumeType.ForceField:
//                        //    CommandBuffer.AddComponent<ForceFieldOverlappingTriggerVolume>(overlappingEntity, new ForceFieldOverlappingTriggerVolume());
//                        //    break;
//                        case TriggerVolumeType.None:
//                        default:
//                            break;
//                    }
//                }
//            }
//        }
//    }

//    //[BurstCompile]
//    struct RemoveOldOverlappingJob : IJob
//    {
//        public EntityCommandBuffer CommandBuffer;

//        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> OverlappingEntities;
//        [ReadOnly] public ComponentDataFromEntity<OverlappingTriggerVolume> OverlappingGroup;
//        [ReadOnly] public ComponentDataFromEntity<TriggerVolume> TriggerGroup;

     

//        public void Execute()
//        {

//            for (int index = 0; index < OverlappingEntities.Length; index++)
//            {
//                //  Debug.Log($"RemoveOldOverlappingJob");

//                var entity = OverlappingEntities[index];
//                var overlappingComponent = OverlappingGroup[entity];


//                int volumeFrame = 0;
//                int overlappingFrame =  overlappingComponent.CurrentFrame;                

//                if (TriggerGroup.Exists(overlappingComponent.VolumeEntity))
//                {
//                    var triggerComponent = TriggerGroup[overlappingComponent.VolumeEntity];
//                    volumeFrame = triggerComponent.CurrentFrame;                   
//                }
        

//                //  Debug.Log($"entity:{entity.Index},volumeFrame:{volumeFrame},overlappingFrame:{overlappingFrame}");

//                // If the frame count of this overlapping entity does not match
//                // the frame count of the associate trigger volume then it was not
//                // in the TriggerEvents list and so must have left that region.
//                if (overlappingFrame != volumeFrame)
//                {
//                     Debug.Log($"RemoveOldOverlappingJob,entity:{entity.Index},overlappingFrame:{overlappingFrame}" +
//                        $",thread:{Thread.CurrentThread.ManagedThreadId}");

//                    switch ((TriggerVolumeType)overlappingComponent.VolumeType)
//                    {
//                        case TriggerVolumeType.Destroy:
//                            CommandBuffer.RemoveComponent<DestroyOverlappingTriggerVolume>(entity);
//                            break;
//                        case TriggerVolumeType.ReduceLife:
//                            CommandBuffer.RemoveComponent<ReduceLifeOverlappingTriggerVolume>(entity);
//                            break;
//                        case TriggerVolumeType.DestoryReduceLife:
//                            CommandBuffer.RemoveComponent<DestroyOverlappingTriggerVolume>(entity);
//                            CommandBuffer.RemoveComponent<ReduceLifeOverlappingTriggerVolume>(entity);
//                            break;
//                        //case TriggerVolumeType.ForceField:
//                        //    CommandBuffer.RemoveComponent<ForceFieldOverlappingTriggerVolume>(entity);
//                        //    break;
//                        case TriggerVolumeType.None:
//                        default:
//                            break;
//                    }
//                    CommandBuffer.RemoveComponent<OverlappingTriggerVolume>(entity);
//                }
//            }
//        }
//    }

//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
        
//        JobHandle jobHandle;

//        // Increment the frame count on all active TriggerVolume components
//        JobHandle updateVolumesJobHandle = new UpdateTriggerVolumesJob().Schedule(this, inputDeps);

//        // Get the number of TriggerEvents so that we can allocate a native array
//        m_TriggerEntitiesIndex[0] = 0;
//        JobHandle getTriggerEventCountJobHandle = new GetTriggerEventCount()
//        {
//            pCounter = m_TriggerEntitiesIndex,
//        }.Schedule(m_StepPhysicsWorldSystem.Simulation, ref m_BuildPhysicsWorldSystem.PhysicsWorld, inputDeps);
//        getTriggerEventCountJobHandle.Complete();

//        // Get the list of overlapping bodies
//        var triggerEntities = new NativeArray<TriggerEntities>(m_TriggerEntitiesIndex[0], Allocator.TempJob);
//        m_TriggerEntitiesIndex[0] = 0;
//        JobHandle listTriggerEventEntitiesJobHandle = new ListTriggerEventEntitiesJob
//        {
//            PhysicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(true),
//            VolumeGroup = GetComponentDataFromEntity<TriggerVolume>(true),
//            TriggerEntities = triggerEntities,
//            pCounter = m_TriggerEntitiesIndex,
//        }.Schedule(m_StepPhysicsWorldSystem.Simulation, ref m_BuildPhysicsWorldSystem.PhysicsWorld, updateVolumesJobHandle);

//        // Increment the frame count on the OverlappingTriggerVolume components
//        // that are still present in the TriggerEvent list
//        var overlappingGroup = GetComponentDataFromEntity<OverlappingTriggerVolume>();
//        JobHandle updateOverlappingJobHandle = new UpdateOverlappingJob
//        {
//            TriggerEntites = triggerEntities,
//            TriggerEntitiesCount = m_TriggerEntitiesIndex,
//            OverlappingGroup = overlappingGroup,
//        }.Schedule(listTriggerEventEntitiesJobHandle);

//        // Add OverlappingTriggerVolume components to any Entity that has
//        // just entered a trigger volume
//        var triggerGroup = GetComponentDataFromEntity<TriggerVolume>(true);
//        JobHandle addNewJobHandle = new AddNewOverlappingJob
//        {
//            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
//            TriggerEntities = triggerEntities,
//            TriggerEntitiesCount = m_TriggerEntitiesIndex,
//            TriggerGroup = triggerGroup,
//            OverlappingGroup = overlappingGroup,
//        }.Schedule(updateOverlappingJobHandle);
//        m_EntityCommandBufferSystem.AddJobHandleForProducer(addNewJobHandle);

//        // Remove the OverlappingTriggerVolume component from any Entity
//        // that did not have its frame count incremented and so will be behind
//        // the frame count of the associated trigger. i.e. it has left the trigger volume
//        JobHandle removeOldJobHandle = new RemoveOldOverlappingJob
//        {
//        //    EntityManager = EntityManager,
//            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
//            OverlappingEntities = OverlappingGroup.ToEntityArray(Allocator.TempJob),
//            TriggerGroup = triggerGroup,
//            OverlappingGroup = overlappingGroup,
//        }.Schedule(updateOverlappingJobHandle);
//        m_EntityCommandBufferSystem.AddJobHandleForProducer(removeOldJobHandle);

//        jobHandle = JobHandle.CombineDependencies( addNewJobHandle, removeOldJobHandle );

//        return jobHandle;
//    }
//}