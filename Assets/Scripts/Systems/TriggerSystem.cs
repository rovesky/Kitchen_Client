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
   
    // This system applies an impulse to any dynamic that collides with a Repulsor.
    // A Repulsor is defined by a PhysicsShape with the `Raise Collision Events` flag ticked and a
    // CollisionEventImpulse behaviour added.
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class TriggerSystem : JobComponentSystem
    {
        BuildPhysicsWorld m_BuildPhysicsWorldSystem;
        StepPhysicsWorld m_StepPhysicsWorldSystem;
        EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
        NativeArray<int> m_TriggerEntitiesIndex;

        protected override void OnCreate()
        {

            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
            m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            m_TriggerEntitiesIndex = new NativeArray<int>(2, Allocator.Persistent);
            m_TriggerEntitiesIndex[1] = 0;
    
        }

        // [BurstCompile]
        struct TriggerEventJob : ITriggerEventsJob
        {
            public EntityCommandBuffer CommandBuffer;
            [ReadOnly] public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityGroup;
            [ReadOnly] public ComponentDataFromEntity<TriggerDestroy> DestroyGroup;

            //public ComponentDataFromEntity<PhysicsCollider> PhysicsColliderGroup;

            public ComponentDataFromEntity<Attack> AttackGroup;
            public ComponentDataFromEntity<Damage> DamageGroup;
           

            //  public ComponentDataFromEntity<TriggerTimeoutFrame> TriggerTimeoutFrameGroup;
            [NativeFixedLength(2)] public NativeArray<int> pCounter;
       

            public void Execute(TriggerEvent triggerEvent)
            {
                //  Debug.Log($"pCounter1[0]:{pCounter1[0]% 2},pCounter[0]:{pCounter[0]}!");
                if (pCounter[1] % 2 == 0)
                    return;
                if (pCounter[0] > 0)
                    return;

                pCounter[0]++;

                Entity entityA = triggerEvent.Entities.EntityA;
                Entity entityB = triggerEvent.Entities.EntityB;          

                if (DestroyGroup.Exists(entityA))
                {
                    CommandBuffer.DestroyEntity(entityA);
                }
                if (DestroyGroup.Exists(entityB))
                {
                    CommandBuffer.DestroyEntity(entityB);
                }
                               
                bool isBodyAAttacker = AttackGroup.Exists(entityA);
                bool isBodyBAttacker = AttackGroup.Exists(entityB);

                bool isBodyADamage = DamageGroup.Exists(entityA);
                bool isBodyBDamage = DamageGroup.Exists(entityB);


                if (isBodyAAttacker && isBodyBDamage)
                {
                    var damageComponent = DamageGroup[entityB];
                    damageComponent.Value += AttackGroup[entityA].Power;
                    DamageGroup[entityB] = damageComponent;
                }

                if (isBodyBAttacker && isBodyADamage)
                {
                    var damageComponent = DamageGroup[entityA];
                    damageComponent.Value += AttackGroup[entityB].Power;
                    DamageGroup[entityA] = damageComponent;
                }
            }
        }             

        protected override void OnDestroy()
        {
            m_TriggerEntitiesIndex.Dispose();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            m_TriggerEntitiesIndex[0] = 0;
            m_TriggerEntitiesIndex[1]++;           

        //    Debug.Log($" m_TriggerEntitiesIndex1[0]:{ m_TriggerEntitiesIndex1[0]}, m_TriggerEntitiesIndex[0]:{ m_TriggerEntitiesIndex[0]}!");
            JobHandle jobCollisionEventEnemy = new TriggerEventJob
            {
                pCounter = m_TriggerEntitiesIndex,      
                CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
                PhysicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(true),

                DestroyGroup = GetComponentDataFromEntity<TriggerDestroy>(true),
                DamageGroup = GetComponentDataFromEntity<Damage>(),        
                AttackGroup = GetComponentDataFromEntity<Attack>(),
          

            }.Schedule(m_StepPhysicsWorldSystem.Simulation,
                        ref m_BuildPhysicsWorldSystem.PhysicsWorld, inputDeps);
            jobCollisionEventEnemy.Complete();
          
            return jobCollisionEventEnemy;
        }
    }
}
