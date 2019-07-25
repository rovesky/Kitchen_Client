using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    //public struct Collision : IComponentData
    //{
    //    public bool IsDestroy;

    //    public bool IsCollision;
        
    //}

    //public class CollisionBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    //{
    //    public bool IsDestroy;
    //    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    //    {
    //        dstManager.AddComponentData(entity, new Collision() {
    //            IsDestroy = IsDestroy,
    //            IsCollision = false });
    //    }
    //}


    // This system applies an impulse to any dynamic that collides with a Repulsor.
    // A Repulsor is defined by a PhysicsShape with the `Raise Collision Events` flag ticked and a
    // CollisionEventImpulse behaviour added.
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class CollisionEnemySystem : JobComponentSystem
    {
        BuildPhysicsWorld m_BuildPhysicsWorldSystem;
        StepPhysicsWorld m_StepPhysicsWorldSystem;
        EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

        protected override void OnCreate()
        {

            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
            m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        }

        // [BurstCompile]
        struct CollisionEventImpulseJob : ITriggerEventsJob
        {
            public EntityCommandBuffer CommandBuffer;
            public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityGroup;
            public ComponentDataFromEntity<PhysicsCollider> PhysicsColliderGroup;

            public ComponentDataFromEntity<Life> LifeGroup;
            public ComponentDataFromEntity<Attack> AttackGroup;
            public ComponentDataFromEntity<CollisionDestroy> CollisionDestroyGroup;

            public void Execute(TriggerEvent triggerEvent)
            {

                Entity entityA = triggerEvent.Entities.EntityA;
                Entity entityB = triggerEvent.Entities.EntityB;

                bool isBodyAAttacker = AttackGroup.Exists(entityA);
                bool isBodyBAttacker = AttackGroup.Exists(entityB);

                if (isBodyAAttacker && isBodyBAttacker)
                    return;

                bool isBodyALife = LifeGroup.Exists(entityA);
                bool isBodyBLife = LifeGroup.Exists(entityB);

                // Ignoring overlapping static bodies
                if ((isBodyAAttacker && !isBodyBLife) ||
                    (isBodyBAttacker && !isBodyBLife))
                    return;

                var attckerEntity = isBodyAAttacker ? entityA : entityB;
                var lifeEntity = isBodyALife ? entityA : entityB;
                               

                //攻击方被删除
                if (CollisionDestroyGroup.Exists(attckerEntity))
                {
                    if (CollisionDestroyGroup[attckerEntity].IsCollision)
                        return;

                    var attackComponent = CollisionDestroyGroup[attckerEntity];
                     attackComponent.IsCollision = true;
                    CollisionDestroyGroup[attckerEntity] = attackComponent;
                    CommandBuffer.DestroyEntity(attckerEntity);
                }

                //攻击方损失生命
                if (LifeGroup.Exists(attckerEntity))
                {
                    var attackerLife = LifeGroup[attckerEntity];
                    attackerLife.lifeValue -= 1;
                    LifeGroup[attckerEntity] = attackerLife;
                }

                if (AttackGroup.Exists(attckerEntity))
                {
                    //扣除生命
                    var lifeComponent = LifeGroup[lifeEntity];
                    lifeComponent.lifeValue -= AttackGroup[attckerEntity].Power;
                    LifeGroup[lifeEntity] = lifeComponent;
                    Debug.Log($"triggerEvent:{lifeComponent.lifeValue}");
                } 
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            JobHandle jobHandle = new CollisionEventImpulseJob
            {
                CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
                PhysicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(),
                PhysicsColliderGroup = GetComponentDataFromEntity<PhysicsCollider>(),
                CollisionDestroyGroup = GetComponentDataFromEntity<CollisionDestroy>(),
                LifeGroup = GetComponentDataFromEntity<Life>(),
                AttackGroup = GetComponentDataFromEntity<Attack>(),

            }.Schedule(m_StepPhysicsWorldSystem.Simulation,
                        ref m_BuildPhysicsWorldSystem.PhysicsWorld, inputDeps);

            m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

            return jobHandle;
        }
    }

}
