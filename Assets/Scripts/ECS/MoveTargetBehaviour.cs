using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [Serializable]
    public struct MoveTarget : IComponentData
    {
        public int Speed;      
    }

    public class MoveTargetBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public int Speed; 

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (enabled)
            {
                dstManager.AddComponentData(
                    entity,
                    new MoveTarget()
                    {
                        Speed = Speed,
                    });
            }
        }
    }


    #region System
    // update before physics gets going so that we dont have hazzard warnings
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class MoveTargetSystem : ComponentSystem
    {
        public EntityQuery PlayerGroup;

        protected override void OnCreate()
        {
            PlayerGroup = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                   typeof(Player),
                }
            });
        }

        //struct MoveTargetJob : IJobForEach<Translation, Rotation, MoveTargetComponent>
        //{
        //    [ReadOnly] public ComponentDataFromEntity<Translation> PositionComponents;
        //    [ReadOnly] public ComponentDataFromEntity<Rotation> RotationComponents;

        //    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> PlayerEntities;

        //    public void Execute(ref Translation position, ref Rotation rotation, ref MoveTargetComponent move)
        //    {

        //        if (PlayerEntities.Length == 0)
        //            return;

        //        var target = PlayerEntities[0];



        //        var targetPos = PositionComponents[target];
        //        var targetRotation = RotationComponents[target];

        //        Vector3 value = Vector3.MoveTowards(position.Value, targetPos.Value, move.Speed * Time.deltaTime);

        //        position = new Translation()
        //        {
        //            Value = value
        //        };

        //        Vector3 relativePos = position.Value - targetPos.Value;

        //        rotation = new Rotation()
        //        {
        //            Value = Quaternion.LookRotation(relativePos)
        //        };
        //    }
        //}
        //protected override JobHandle OnUpdate(JobHandle inputDeps)
        //{
        //    var job = new MoveTargetJob
        //    {
        //        PlayerEntities = PlayerGroup.ToEntityArray(Allocator.TempJob),
        //        PositionComponents = GetComponentDataFromEntity<Translation>(true),
        //        RotationComponents = GetComponentDataFromEntity<Rotation>(true),

        //    }.ScheduleSingle(this, inputDeps);

        //    //    m_EntityCommandBufferSystem.AddJobHandleForProducer(job);

        //    return job;
        //}

        protected override void OnUpdate()
        {
            var PlayerEntities = PlayerGroup.ToEntityArray(Allocator.Persistent);

            if (PlayerEntities.Length == 0)
            {
                PlayerEntities.Dispose();
                return;
            }

            Entities.ForEach((ref Translation position, ref Rotation rotation, ref MoveTarget move) =>
            {           

                var target = PlayerEntities[0];

                var targetPos = EntityManager.GetComponentData<Translation>(target);
                var targetRotation = EntityManager.GetComponentData<Rotation>(target);

                Vector3 value = Vector3.MoveTowards(position.Value, targetPos.Value, move.Speed * Time.deltaTime);

                position = new Translation()
                {
                    Value = value
                };

                Vector3 relativePos = position.Value - targetPos.Value;

                rotation = new Rotation()
                {
                    Value = Quaternion.LookRotation(relativePos)
                };
            });
            PlayerEntities.Dispose();
        }
    }
    #endregion

}